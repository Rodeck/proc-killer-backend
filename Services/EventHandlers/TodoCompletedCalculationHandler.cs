using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.EventHandlers
{
    public class TodoCompletedCalculationHandler : IEventCalculationHandler<TodoCompletedEvent>
    {
        public UserState Calculate(TodoCompletedEvent @event, UserState currentState)
        {
            currentState.Points += SystemSettings.PointsForCompletition;
            @event.Points = SystemSettings.PointsForCompletition;
            currentState.TotalTodosCompleted += 1;

            return currentState;
        }
    }
}
