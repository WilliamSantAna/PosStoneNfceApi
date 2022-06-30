using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class ProdutoRequest : IRequest
    {
        public string stone_code;
        public string produto_id;
    }
}