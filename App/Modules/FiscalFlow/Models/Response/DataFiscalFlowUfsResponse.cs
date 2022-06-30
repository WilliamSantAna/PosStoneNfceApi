namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response
{
    public class DataFiscalFlowUfsResponse
    {
        public int CodigoIbge;
        public string Sigla;
        public string Nome;

        public DataFiscalFlowUfsResponse(int CodigoIbge, string Sigla, string Nome) {
            this.CodigoIbge = CodigoIbge;
            this.Sigla = Sigla;
            this.Nome = Nome;
        }
    }
}