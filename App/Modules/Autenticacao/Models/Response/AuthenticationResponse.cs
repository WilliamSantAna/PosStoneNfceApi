namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models.Response
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string UsuarioId { get; set; }
        public AuthenticationHeaders Dados { get; set; }        
    }
}