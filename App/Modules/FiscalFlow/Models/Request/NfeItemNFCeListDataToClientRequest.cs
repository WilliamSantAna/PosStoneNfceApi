namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfeItemNFCeListDataToClientRequest
    {
        public int nItem { get; set; }
        public string cProd { get; set; }
        public string xProd { get; set; }
        public string cEAN { get; set; }
        public string cEANTrib { get; set; }
        public string NCM { get; set; }
        public int CFOP { get; set; }
        public string uCom { get; set; }
        public string uTrib { get; set; }
        public double qCom { get; set; }
        public double qTrib { get; set; }
        public double vUnCom { get; set; }
        public double vProd { get; set; }
        public int orig { get; set; }
        public int modBC { get; set; }
        public double vBC { get; set; }
        public double pICMS { get; set; }
        public double vICMS { get; set; }
        public string CST_CSOSN { get; set; }
        public string CST_PIS { get; set; }
        public string CST_COF { get; set; }

    }
}