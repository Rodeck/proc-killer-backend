using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class ActivationModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Code { get; set; }

        public DateTime GenerationDate { get; set; }
    }
}
