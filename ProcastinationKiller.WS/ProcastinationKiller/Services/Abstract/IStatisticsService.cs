using ProcastinationKiller.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    /// <summary>
    /// Interfejs dla usługi wyliczającej statystyki
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// Wyznacza ilość punktów zdobywanych każdego dnia
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PointsPerDayModel[] CalculatePointsPerDay(int userId); 
    }
}
