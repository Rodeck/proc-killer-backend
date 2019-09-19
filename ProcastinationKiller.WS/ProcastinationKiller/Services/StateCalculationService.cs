using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using ProcastinationKiller.Services.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class StateCalculationService : IStateCalculationService
    {
        private Dictionary<Type, Func<BaseEvent, UserState, UserState>> _handlers = new Dictionary<Type, Func<BaseEvent, UserState, UserState>>();
        private IDefinitionsContext _definitionsContext;

        public StateCalculationService(IDefinitionsContext definitionsContext)
        {
            RegisterHandlers();
            _definitionsContext = definitionsContext;
        }

        private void RegisterHandlers()
        {
            AddHandler<TodoCompletedEvent, TodoCompletedCalculationHandler>();
            AddHandler<DailyLoginEvent, DailyLoginCalculationHandler>();
            AddHandler<WeeklyLoginEvent, WeeklyLoginCalculationHandler>();
        }

        private void AddHandler<TEvent, THandler>()
            where THandler: IEventCalculationHandler<TEvent>, new()
            where TEvent: BaseEvent
        {
            var handler = (THandler)Activator.CreateInstance<THandler>();
            _handlers.Add(typeof(TEvent), (operation, state) => handler.Calculate((TEvent)operation, state));
        }

        public UserState Calculate(IEnumerable<BaseEvent> baseEvents, DateTime currentTime)
        {
            // Posortuj eventy w kolejności wystąpienia

            var eventQueue = baseEvents.OrderBy(x => x.Date);

            BaseEvent lastOperation = null;
            UserState currentState = null;
            foreach(var @event in eventQueue)
            {
                var handler = _handlers[@event.GetType()];
                @event.State = handler(@event, UserState.Copy(lastOperation?.State ?? new UserState()));
                lastOperation = @event;

                if (@event.Date <= currentTime)
                {
                    AssignLevel(@event.State, @event);
                    currentState = @event.State;
                }
            }

            return currentState;
        }

        private UserState AssignLevel(UserState userState, BaseEvent baseEvent)
        {
            var level = userState.Level;

            if (level != null)
            {
                // Level up
                if (level.RequiredExp <= baseEvent.Points)
                {
                    var currentLevelDefinition = _definitionsContext.GetLevel(level.Number + 1);

                    userState.Level = new Level()
                    {
                        Definition = currentLevelDefinition,
                        Number = level.Number + 1,
                        CurrentExp = baseEvent.Points.GetValueOrDefault() - level.RequiredExp,
                        RequiredExp = level.RequiredExp = currentLevelDefinition.RequiredExp
                    };
                }
                else
                {
                    level.CurrentExp += baseEvent.Points.GetValueOrDefault();
                }
            }
            else
            {
                var currentLevelDefinition = _definitionsContext.GetLevel(1);
                userState.Level = new Level()
                {
                    Number = 1,
                    CurrentExp = baseEvent.Points.GetValueOrDefault(),
                    RequiredExp = currentLevelDefinition.RequiredExp,
                    Definition = currentLevelDefinition
                };
            }

            return userState;
        }
    }
}
