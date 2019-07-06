using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    public interface IMailingService
    {
        Task SendEmail(string body, string to);
    }
}
