using System.Threading.Tasks;

namespace PosStoneNfce.Common.Modules.Email
{
    public interface IEmailService
    {
        EmailMessage ConvertTemplateToDefault(EmailTemplateMessage email);
        Task Send(EmailTemplateMessage emailMessage);
        Task Send(IEmailFactory emailFactory);
        void SetTransporter(IEmailTransporter emailTransporter);
    }
}