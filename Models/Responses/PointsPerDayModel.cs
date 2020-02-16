using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models.Responses
{
    public class PointsPerDayModel
    {
        public DateTime Date { get; set; }

        public int Points { get; set; }
    }

    public class TodosCumulativeModel
    {
        public DateTime Date { get; set; }

        public int TotalTodos { get; set; }
    }

    public class CumulativeResult
    {
        public DateTime Date { get; set; }

        public int All { get; set; }

        public int Completed { get; set; }

        public int NotCompleted { get; set; }
    }
}
