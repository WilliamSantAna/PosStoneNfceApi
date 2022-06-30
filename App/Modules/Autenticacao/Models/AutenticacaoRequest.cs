namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models
{
    public class AutenticacaoRequest
    {
        public AutenticacaoRequest(Input input)
        {
            Input = input;
        }

        public Input Input { get; set; }
    }
}