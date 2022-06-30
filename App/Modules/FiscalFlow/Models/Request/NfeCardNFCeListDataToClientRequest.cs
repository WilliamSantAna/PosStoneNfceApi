namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfeCardNFCeListDataToClientRequest
    {
        public string CNPJx { get; set; }
        public int tBand { get; set; }
        public string cAut { get; set; }
        public string tpIntegra { get; set; }
    }
}