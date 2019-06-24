using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models.Responses
{
    public class EventModel
    {
        public int Id { get; set; }

        public int Points { get; set; }

        public int PointsAfterEvent { get; set; }

        public DateTime EventDate { get; set; }

        public string EventType { get; set; }
    }
}
