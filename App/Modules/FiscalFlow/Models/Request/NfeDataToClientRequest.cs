using PosStoneNfce.API.Portal.Interfaces;
using System.Collections.Generic;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request
{
    public class NfeDataToClientRequest : IRequest
    {
        public bool GeraDanfe { get; set; }
        public bool GeraDanfe56mm { get; set; }
        public int ExtensaoDanfe { get; set; }
        public bool DevolveXml { get; set; }
        public int tipoemissao { get; set; }
        public string versao { get; set; }
        public string natOp { get; set; }
        public string serie { get; set; }
        public int nNF { get; set; }
        public string dhEmi { get; set; }
        public int tpNF { get; set; }
        public int idDest { get; set; }
        public int tpImp { get; set; }
        public int tpEmis { get; set; }
        public int finNFe { get; set; }
        public int tpAmb { get; set; }
        public int IndFinal { get; set; }
        public int IndPres { get; set; }
        public string CNPJ { get; set; }
        public string CRT { get; set; }
        public string TIPOIDCONS { get; set; }
        public string IDCONS { get; set; }
        public string xNome { get; set; }
        public string indIEDest { get; set; }
        public double vProd { get; set; }
        public double vNF { get; set; }
        public double vIPI { get; set; }
        public double vPIS { get; set; }
        public double vCOFINS { get; set; }
        public double vBCST { get; set; }
        public double vST { get; set; }
        public int idProduto { get; set; }
        public List<NfeItemNFCeListDataToClientRequest> ItemNFCeList { get; set; }
        public List<NfePagNFCeListDataToClientRequest> PagNFCeList {get; set; }
    }
}