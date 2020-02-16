using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class MailProvider : IMailProvider
    {
        private ITemplateProvider _templateProvider;

        public MailProvider(ITemplateProvider templateProvider)
        {
            _templateProvider = templateProvider;
        }

        public async Task<Mail> GetRegistrationMailBody(string registartionCode)
        {
            var template = new RegistrationTemplate(_templateProvider);

            return await template.FillParams(new Param()
            {
                Key = "link",
                Value = registartionCode
            })
            .BuildAsync();
        }
    }
}
