using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    /// <summary>
    /// Warunek uzyskania nagrody.
    /// </summary>
    public class RewardCondition
    {
        /// <summary>
        /// Id warunku
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Typ warunku jaki musi być spełniony.
        /// </summary>
        public Conditions Condition { get; set; }

        /// <summary>
        /// Ilość jaka musi być spełniona.
        /// </summary>
        public int Amount { get; set; }
    }

    /// <summary>
    /// Enum reprezentuje wszystkie możliwe warunki.
    /// </summary>
    public enum Conditions
    {
        /// <summary>
        /// Ilość logowań.
        /// </summary>
        Logins = 1,

        /// <summary>
        /// Ilość ukończonych todo.
        /// </summary>
        TodosCompleted = 2,

        /// <summary>
        /// Ilość zdobytych punktów.
        /// </summary>
        PointsEarned = 3
    }
}
