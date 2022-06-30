using System;
using System.Net.Http;
using System.Linq;
using PosStoneNfce.API.Portal.App.Common.Utils;
using System.Threading.Tasks;


namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services
{
    public class CurlService
    {
        static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> Execute<T>(string url, T data, string bearer)
        {
            string result = "";
            HttpResponseMessage httpResponseMessage;

            if (bearer is not null) {
                if (httpClient.DefaultRequestHeaders.Any()) {
                    httpClient.DefaultRequestHeaders.Clear();
                }
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearer}");            
            }

            if (data is not null) {
                // É um metodo post
                //StringContent stringContent = new StringContent("");
                StringContent stringContent = ObjectExtensions.ToJSONStringContent(data);
                httpResponseMessage = await httpClient.PostAsync(new Uri(url), stringContent);
            }
            else {
                // É um metodo get
                httpResponseMessage = await httpClient.GetAsync(new Uri(url));
            }

            result = await httpResponseMessage.Content.ReadAsStringAsync();
            return result;
        }

        public static string Post(string url, object data, string bearer) 
        {
            string res = Execute(url, data, bearer).Result;
            return res;
        }

        public static string TypePost<T>(string url, T data, string bearer) 
        {
            string res = Execute<T>(url, data, bearer).Result;
            return res;
        }


        public static string Get(string url, string bearer) 
        {
            string res = Execute<object>(url, null, bearer).Result;
            return res;
        }

        public static string GetPayload(string url, object data, string bearer) 
        {
            string payload = "curl --location --request " + (data is not null ? "POST" : "GET") + " '" + url + "' ";
            if (bearer is not null) {
                payload += "--header 'Authorization: Bearer " + bearer + "' ";
            }
            if (data is not null) {
                payload += "--header 'Content-Type: application/json' ";
                payload += "--data-raw '" + ObjectExtensions.ToJSON(data) + "'";
            }
            return payload;
        }

        public static string GetTypePayload<T>(string url, T data, string bearer) 
        {
            string payload = "curl --location --request " + (data is not null ? "POST" : "GET") + " '" + url + "' ";
            if (bearer is not null) {
                payload += "--header 'Authorization: Bearer " + bearer + "' ";
            }
            if (data is not null) {
                payload += "--header 'Content-Type: application/json' ";
                payload += "--data-raw '" + ObjectExtensions.ToJSON(data) + "'";
            }
            return payload;
        }
    }
}