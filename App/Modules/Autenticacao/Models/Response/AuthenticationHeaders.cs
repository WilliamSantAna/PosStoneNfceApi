namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models.Response
{
    public class AuthenticationHeaders
    {
        public string Application { get; set; }
        public string CurrentCompany { get; set; }
        public string AuthorizationToken { get; set; }
        public string CurrentUser { get; set; }
        public string AccessGroup { get; set; }
        public string DataCriacao { get; set; }
    }    
}