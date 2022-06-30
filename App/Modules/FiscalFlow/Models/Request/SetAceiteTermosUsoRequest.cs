using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class SetAceiteTermosUsoRequest : IRequest
    {
        public string stone_code;
        public string aceite_termos_uso;
    }
}