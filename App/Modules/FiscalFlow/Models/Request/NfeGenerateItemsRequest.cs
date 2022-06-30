using PosStoneNfce.API.Portal.Interfaces;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfeGenerateItemsRequest: IRequest
    {
        
        public string id;
        public string qty;
        public string vlr;
    }
}