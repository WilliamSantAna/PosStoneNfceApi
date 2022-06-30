using System.ComponentModel.DataAnnotations;

namespace PosStoneNfce.API.Portal.App.Common.Utils.Request
{
    public class FrontRequest<T>
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public T Data { get; set; }        
    }
}