namespace PosStoneNfce.Common.Modules.Email
{
    public interface IEmailFactory
    {
        EmailTemplateMessage GetEmail();
    }
}