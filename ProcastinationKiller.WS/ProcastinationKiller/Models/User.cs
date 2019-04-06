using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public DateTime Regdate { get; set; }

        public ICollection<TodoItem> UserTodos { get; set; }

        public User()
        {
            UserTodos = new HashSet<TodoItem>();
            _days = _days ?? new HashSet<Day>();
        }

        [NotMapped]
        public ICollection<Day> Callendar
        {
            get
            {
                InitializeCallendar();
                return _days;
            }
            private set
            {

            }
        }

        private ICollection<Day> _days;

        private void InitializeCallendar()
        {
            foreach(var todo in UserTodos)
            {
                Day day = _days.SingleOrDefault(x => x.Date.Date == todo.TargetDate.Date);

                if (day == null)
                {
                    day = new Day();
                    day.Date = todo.TargetDate;
                    _days.Add(day);
                }

                if (day.Todos == null)
                {
                    day.Todos = new HashSet<TodoItem>();
                }

                day.Todos.Add(todo);

                if (day.AllCompleted && !todo.Completed)
                    day.AllCompleted = false;
            }
        }
    }
}
