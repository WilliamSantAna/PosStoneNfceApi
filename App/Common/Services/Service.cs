using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PosStoneNfce.API.Portal.App.Common.Services
{
    public abstract class Service
    {
        protected StringContent GetContent(object data)
        {
            var stringContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            return stringContent;
        }        

        protected async Task<T> DeserializeResponseObject<T>(HttpResponseMessage httpResponseMessage)
        {
            var responseObject = JsonConvert.DeserializeObject<T>(
                await httpResponseMessage.Content.ReadAsStringAsync());

            return responseObject;
        }           
    }
}