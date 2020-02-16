using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Exceptions
{
    public class ProcSystemException: Exception
    {
        public ProcSystemException(string message)
            :base(message)
        {

        }
    }
}
