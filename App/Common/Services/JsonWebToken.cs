namespace PosStoneNfce.API.Portal.App.Common.Services
{
    public class JsonWebToken
    {
        public string Secret { get; set; }
        public int ExpirationHours { get; set; }
        public string Emissor { get; set; }
        public string ValidoEm { get; set; }
    }
}