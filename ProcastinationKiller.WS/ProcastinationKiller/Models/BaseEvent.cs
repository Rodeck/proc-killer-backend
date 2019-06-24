using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public abstract class BaseEvent
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public bool Hidden { get; set; }

        public UserState State { get; set; }

        public int? Points { get; set; }
    }

    public class TodoCompletedEvent: BaseEvent
    {
        public TodoItem CompletedItem { get; set; }
    }

    public class DailyLoginEvent: BaseEvent
    {

    }

    public class WeeklyLoginEvent: BaseEvent
    {

    }
}
