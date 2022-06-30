using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class StoneCodeRequest : IRequest
    {
        public string stone_code;
        public string user;
        public string pass;
    }
}