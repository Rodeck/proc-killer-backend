using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class LevelDefinition
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int RequiredExp { get; set; }

        public League League { get; set; }
    }
}
