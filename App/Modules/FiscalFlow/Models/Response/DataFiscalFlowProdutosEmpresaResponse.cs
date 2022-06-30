namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response
{
    public class DataFiscalFlowProdutosEmpresaResponse
    {
        public string Id; 
        public string NomeComercial; 
        public double Preco; 
        public string Custo; 
        public string CFOP;
        public string Codigo;
        public string Tags; 
        public string ProdutoId; 
        public string EmpresaCNPJ; 
        public string UnidadeMedidaId; 
        public string CriadoEm; 
        public DataFiscalFlowProdutoEmpresaProdutoResponse Produto;
        public DataFiscalFlowProdutoEmpresaImpostosResponse Impostos;
    }
}