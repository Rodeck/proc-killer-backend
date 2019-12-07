using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class TodoInputModel
    {
        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Opis
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data na jaka jest todo
        /// </summary>
        public DateTime TargetDate { get; set; }
    }

    public class TodoCompleteInputModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Id użytkownika
        /// </summary>
        public string UserId { get; set; }
    }
}
