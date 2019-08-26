using Microsoft.Extensions.Options;
using ProcastinationKiller.Models;
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
        private MailAddress From { get; set; }

        private string Password { get; set; }

        public MailingService(IOptions<MailingOptions> options)
        {
            From = new MailAddress(options.Value.Address, options.Value.Address);
            Password = options.Value.Password;
        }

        public async Task SendEmail(Mail mail, string to)
        {

            var toAddress = new MailAddress(to);

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(From.Address, Password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            using (var message = new MailMessage(From, toAddress)
            {
                Subject = "ToDo",
                Body = mail.Body,
                IsBodyHtml = mail.IsHtml
            })
            {
                await client.SendMailAsync(message);
            }  
        }
    }
}
