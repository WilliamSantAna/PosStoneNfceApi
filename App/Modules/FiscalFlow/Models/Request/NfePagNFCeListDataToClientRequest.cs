using System.Collections.Generic;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfePagNFCeListDataToClientRequest
    {
        public int tPag { get; set; }
        public double vPag { get; set; }
        public List<NfeCardNFCeListDataToClientRequest> CardNFCeList { get; set; }
    }
}