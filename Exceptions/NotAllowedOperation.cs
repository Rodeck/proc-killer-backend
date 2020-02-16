using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Exceptions
{
    public class NotAllowedOperation: ProcSystemException
    {
        public NotAllowedOperation(string message)
            :base(message)
        {

        }
    }
}
