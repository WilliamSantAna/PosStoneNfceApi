namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models
{
    public class Input
    {
        public Input(string id)
        {
            Nome = id;
        }

        public string Nome { get; set; }
    }       
}