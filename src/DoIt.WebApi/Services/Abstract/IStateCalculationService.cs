using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    /// <summary>
    /// Interefejs dla usługi przeliczającej operacje
    /// </summary>
    public interface IStateCalculationService
    {
        /// <summary>
        /// Przelicz kolekcję operacji zwracając stan
        /// </summary>
        /// <param name="baseEvents"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        UserState Calculate(IEnumerable<BaseEvent> baseEvents, DateTime currentTime);
    }
}
