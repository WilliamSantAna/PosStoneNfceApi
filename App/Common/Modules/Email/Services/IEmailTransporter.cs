using System.Threading.Tasks;

namespace PosStoneNfce.Common.Modules.Email
{
    public interface IEmailTransporter
    {
        Task Send(EmailMessage emailMessage);
    }
}