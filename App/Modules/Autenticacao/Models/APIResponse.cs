namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models
{
    public class APIResponse<T>
    {
        public APIResponse(){}

        public APIResponse(string errors)
        {
            Sucesso = false;
            Errors = errors;
        }

        public T Data { get; set; }

        public string Code { get; set; }

        public string Errors { get; set; }       

        public bool Sucesso { get; set; }
    }
}