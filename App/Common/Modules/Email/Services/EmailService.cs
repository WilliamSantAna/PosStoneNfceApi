
using PosStoneNfce.API.Portal.App.Common.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PosStoneNfce.Common.Modules.Email
{

    public class EmailService : IEmailService
    {
        private readonly IHostEnvironment _hostEnvironment;

        private IEmailTransporter _emailTransporter;
        
        string templatesPath;

        public EmailService(
            IHostEnvironment hostEnvironment
        )
        {
            _hostEnvironment = hostEnvironment;
            templatesPath = @"App\System\Files\email-templates\";
        }


        public void SetTransporter(IEmailTransporter emailTransporter)
        {
            _emailTransporter = emailTransporter;
        }


        public Task Send(IEmailFactory emailFactory)
        {
            var email = this.ConvertTemplateToDefault(emailFactory.GetEmail());
            return _emailTransporter.Send(email);
        }


        public Task Send(EmailTemplateMessage emailTemplate)
        {
            var email = this.ConvertTemplateToDefault(emailTemplate);
            return _emailTransporter.Send(email);
        }


        public EmailMessage ConvertTemplateToDefault(EmailTemplateMessage email)
        {
            try
            {
                string subPath = string.Concat(templatesPath, email.Template, @".template.txt");
                string filePath = Path.Combine(_hostEnvironment.ContentRootPath, subPath);
                string template = File.ReadAllText(filePath).Trim();
                // _telemetryClient.TrackEvent($"Caminho do ContentRootPath: {filePath}.");

                foreach (KeyValuePair<string, dynamic> item in email.DataBind)
                {
                    template = template.Replace("{{" + item.Key + "}}", item.Value);
                }

                return new EmailMessage
                {
                    Identifier = email.Identifier,
                    Metadata = new EmailMetadata
                    {
                        Body = template,
                        Subject = email.Subject,
                        To = email.To
                    }
                };
            }
            catch (Exception e)
            {
                // _telemetryClient.TrackEvent($"Erro - Envio de Email: {e}.");
                throw new Exception("Erro ao converter template de email para metadados.", e);
            }
        }


    }
}