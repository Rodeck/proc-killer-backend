using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public interface IStateCalculationService
    {
        UserState Calculate(IEnumerable<BaseEvent> baseEvents, DateTime currentTime);
    }

    public class StateCalculationService : IStateCalculationService
    {
        private Dictionary<Type, IEventCalculationHandler<BaseEvent>> _handlers = new Dictionary<Type, IEventCalculationHandler<BaseEvent>>();

        public StateCalculationService()
        {
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            AddHandler<TodoCompletedEvent, TodoCompletedCalculationHandler>();
        }

        private void AddHandler<TEvent, THandler>()
            where THandler: IEventCalculationHandler<TEvent>
            where TEvent: BaseEvent
        {
            _handlers.Add(typeof(TEvent), Activator.CreateInstance<THandler>());
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
                @event.State = handler.Calculate(@event, UserState.Copy(lastOperation?.State ?? new UserState()));
                lastOperation = @event;

                if (@event.Date <= currentTime)
                    currentState = @event.State;
            }

            return currentState;
        }
    }

    public interface IEventCalculationHandler<TEvent>
        where TEvent: BaseEvent
    {
        UserState Calculate(TEvent @event, UserState currentState);
    }

    public class TodoCompletedCalculationHandler: IEventCalculationHandler<TodoCompletedEvent>
    {
        public UserState Calculate(TodoCompletedEvent @event, UserState currentState)
        {
            currentState.Points += SystemSettings.PointsForCompletition;

            return currentState;
        }
    }
}
