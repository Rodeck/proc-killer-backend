using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcastinationKiller.Services;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TodosTests
{
    public class MailingServiceTests
    {

        IMailingService mailingService;

        public MailingServiceTests()
        {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IMailingService, MailingService>();
            services.Configure<MailingOptions>(opt =>
            {
                opt.Address = cfg.GetValue<string>("Mail:Address");
                opt.Password = cfg.GetValue<string>("Mail:Password");
            });

            var sp = services.BuildServiceProvider();
            mailingService = sp.GetRequiredService<IMailingService>();
        }

        [Fact]
        public async void SendMail()
        {
            await mailingService.SendEmail("Hello world!", "pajak.gabriel@gmail.com");
        }
    }
}
