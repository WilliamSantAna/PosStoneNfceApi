using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class SetNfeConfigRequest : IRequest
    {
        public string stone_code;
        public string serie_nfe;
        public string num_nfe;
    }
}