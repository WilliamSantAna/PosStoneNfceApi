using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using PosStoneNfce.API.Portal.App.Common.Services;
using PosStoneNfce.Common.Modules.Email;
using PosStoneNfce.Common.Services;

namespace PosStoneNfce.API.Autenticacao.App.Common.Services
{
    public class MIDeServiceBusEmailTransporter : IEmailTransporter
    {
        private string _emailSbConnection;
        private string _queueName;

        public MIDeServiceBusEmailTransporter(
            string emailSbConnection,
            string queueName
        ) {
            _emailSbConnection = emailSbConnection;
            _queueName = queueName;
        }

        public Task Send(EmailMessage emailMessage)
        {
            string content = JsonSerializer.Serialize(emailMessage);
            return new QueueSender(_emailSbConnection).Send(_queueName, content);
        }
        
    }
}