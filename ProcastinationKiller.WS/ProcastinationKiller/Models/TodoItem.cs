using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ProcastinationKiller.Models
{
    /// <summary>
    /// Klasa reprezentuje rzecz do zrobienia
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Unikalny id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data stworzenia
        /// </summary>
        public DateTime Regdate { get; set; }

        /// <summary>
        /// Data zakończenia
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// Todo planowany na dzień
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Czy zakończony
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Opis
        /// </summary>
        public string Description { get; set; }

        [Required]
        public string TagString
        {
            get { return String.Join(',', _tags); }
            set { _tags = value.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        private IEnumerable<string> _tags { get; set; }

        [NotMapped]
        public IEnumerable<string> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
            }
        }

        /*
        /// <summary>
        /// Id użytkownika
        /// </summary>
        public virtual User User { get; set; }*/

        /// <summary>
        /// Zakończ dane zadanie
        /// </summary>
        public virtual void Finish(DateTime time)
        {
            if (Completed)
                throw new Exception("Todo already completed!");

            Completed = true;

            FinishTime = time;
        }

        /// <summary>
        /// Przywróć dane zadanie do wykonania
        /// </summary>
        public virtual void Undo()
        {
            if (!Completed)
                throw new Exception("Todo not completed, cannot undo!");

            Completed = false;

            FinishTime = null;
        }
    }
}
