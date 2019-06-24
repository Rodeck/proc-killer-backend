using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    /// <summary>
    /// Interfejs do handlera operacji
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventCalculationHandler<in TEvent>
        where TEvent : BaseEvent
    {
        UserState Calculate(TEvent @event, UserState currentState);
    }
}
