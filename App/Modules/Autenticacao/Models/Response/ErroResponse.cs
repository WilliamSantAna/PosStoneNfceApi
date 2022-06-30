namespace PosStoneNfce.API.Portal.App.Modules.Autenticacao.Models.Response
{
    public class ErroResponse
    {
        public string Errors { get; set; }

        public int Code { get; set; }

        public string Reason { get; set; }

        public string TrackId { get; set; }

        public string cStat { get; set; }

        public string xMotivo { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsDomainException { get; set; }

        public object StackTrace { get; set; }
    }
}