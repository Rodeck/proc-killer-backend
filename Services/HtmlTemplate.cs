using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class HtmlTemplate : BaseTemplate
    {
        private ITemplateProvider _templateProvider;

        private Param[] _parameters = new Param[] { };

        public HtmlTemplate(ITemplateProvider templateProvider)
        {
            _templateProvider = templateProvider;
        }

        public static string TemplateFile { get; set; }

        public override BaseTemplate FillParams(params Param[] parameters)
        {
            _parameters = parameters;
            return this;
        }

        public virtual bool ValidateParams()
        {
            return true;
        }

        public override Task<Mail> BuildAsync()
        {
            return this.GetBody();
        }

        public async Task<Mail> GetBody()
        {
            StringBuilder body = new StringBuilder();
            var templateBody = await _templateProvider.GetTemplate(TemplateFile);
            foreach(var param in _parameters)
            {
                body.Append(templateBody.Replace($"<{param.Key}>", param.Value));
            }

            return new Mail()
            {
                Body = body.ToString(),
                IsHtml = true
            };
        }
    }

    public class RegistrationTemplate: HtmlTemplate
    {
        public RegistrationTemplate(ITemplateProvider templateProvider)
            : base(templateProvider)
        {
            TemplateFile = "registration-mail.html";
        }
    }
}
