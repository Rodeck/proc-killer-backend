using System;

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
