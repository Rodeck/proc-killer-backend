using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    /// <summary>
    /// Definicja nagrody w postaci odznaki.
    /// </summary>
    public class BadgeDefinition
    {
        /// <summary>
        /// Id definicji
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obrazek nagrody
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Warunki uzyskania nagrody.
        /// </summary>
        public ICollection<RewardCondition> Conditions { get; set; }

        /// <summary>
        /// Czy nagroda jest dodawana przy rejestracji.
        /// </summary>
        public bool AssignableFormBeggining { get; set; }
    }
}
