using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class Day
    {
        public DateTime Date { get; set; }

        public ICollection<TodoItem> Todos { get; set; }

        public bool AllCompleted { get; set; }

        public Day()
        {
            AllCompleted = true;
        }
    }
}
