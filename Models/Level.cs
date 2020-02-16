using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class Level
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int CurrentExp { get; set; }

        [NotMapped]
        public int RequiredExp
        { 
            get
            {
                return Definition.RequiredExp - CurrentExp;
            }
        }

        public LevelDefinition Definition { get; set; }
    }

    public class League
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
