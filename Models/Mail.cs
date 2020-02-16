using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class Mail
    {
        public bool IsHtml { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
