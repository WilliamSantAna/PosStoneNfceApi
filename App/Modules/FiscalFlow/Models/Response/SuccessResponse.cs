using System.Dynamic;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response
{
    public class SuccessResponse
    {
        public bool error { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string enviroment { get; set; }
        public object data { get; set; }
    }
}