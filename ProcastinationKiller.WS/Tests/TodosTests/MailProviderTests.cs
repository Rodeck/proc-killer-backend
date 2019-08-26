using ProcastinationKiller.Services;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TodosTests
{
    public class MailProviderTests
    {
        private IMailProvider _mailProvider;

        public MailProviderTests()
        {
            ITemplateProvider templateProvider = new FileTemplateProvider();
            _mailProvider = new MailProvider(templateProvider);
        }

        [Theory]
        [InlineData("link", "expected")]
        public async void TestRegistrationTemplate(string param, string expeced)
        {
            var output = await _mailProvider.GetRegistrationMailBody(param);
        }
    }
}
