using Microsoft.Extensions.Options;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class MailingOptions
    {
        public string Address { get; set; }

        public string Password { get; set; }
    }

    public class MailingService : IMailingService
    {
        public string From { get; set; }

        public string Password { get; set; }

        public MailingService(IOptions<MailingOptions> options)
        {
            From = options.Value.Address;
            Password = options.Value.Password;
        }

        public async Task SendEmail(string body, string to)
        {

            var fromAddress = new MailAddress(From, "ProcrastinationKiller");
            var toAddress = new MailAddress(to);

            var message = new MailMessage(fromAddress, toAddress);

            message.Body = body;
            message.Subject = "Todo";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(From, Password),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                
            };

            await client.SendMailAsync(message);
        }
    }
}
