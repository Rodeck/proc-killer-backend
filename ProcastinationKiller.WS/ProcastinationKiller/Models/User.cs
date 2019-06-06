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

        public ICollection<BaseEvent> Events { get; set; }

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

        internal void AddTodoCompletedEvent(TodoItem todo)
        {
            if (Events.OfType<TodoCompletedEvent>().Any(x => x.CompletedItem == todo && !x.Hidden))
            {
                throw new Exception("User already have event about completeing this item.");
            }

            Events.Add(new TodoCompletedEvent()
            {
                Hidden = false,
                Date = todo.FinishTime.Value,
                CompletedItem = todo
            });
        }

        internal void AddDailyLoginReward(DateTime currentTime)
        {
            if (Events.OfType<DailyLoginEvent>().Any(x => x.Date.Date == currentTime.Date && !x.Hidden))
                return;

            Events.Add(new DailyLoginEvent()
            {
                Date = currentTime,
                Hidden = false
            });
        }
    }
}
