using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Buffers;
using System.Net.Http;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data;


namespace PosStoneNfce.API.Portal.App.Common.Utils
{
    public static class ObjectExtensions
    {
        public static T FromJsonStringToObject<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T FromJsonElementToObject<T>(JsonElement element)
        {
            var json = element.GetRawText();
            return FromJsonStringToObject<T>(json);
        }

        public static T GetDataObjectFromJsonPost<T>(JsonElement post) 
        {
            JsonElement data = post.GetProperty("data");
            T dataObject = ObjectExtensions.FromJsonElementToObject<T>(data);
            return dataObject;
        }

        public static string FromDataTableToJSON(DataTable table) {  
            string JSONString = string.Empty;  
            JSONString = JsonConvert.SerializeObject(table);  
            return JSONString;
        }         

        public static dynamic FromDataTableToObject(DataTable table) {  
            string JSONString = string.Empty;  
            JSONString = JsonConvert.SerializeObject(table);  
            var res = JsonConvert.DeserializeObject(JSONString);
            return res;
        }         


        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static object ToObject<T>(T typeClass) {
            object obj = new {};
            PropertyInfo[] properties = typeClass.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var name = property.Name;
                var value = typeClass.GetType().GetProperty(name).GetValue(typeClass);
                Type _type = obj.GetType();
                PropertyInfo _propertyInfo = _type.GetProperty(name);
                _propertyInfo.SetValue(_type, value);
            }
            return obj;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        public static string ToJSON(object data)
        {
            StringContent stringContent = ToJSONStringContent(data);
            string jsonString = stringContent.ReadAsStringAsync().Result;
            return jsonString;
        }
        public static string ToTypeJSON<T>(T data)
        {
            StringContent stringContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize<T>(data),
                Encoding.UTF8,
                "application/json");
            string jsonString = stringContent.ReadAsStringAsync().Result;
            return jsonString;
        }

        public static StringContent ToJSONStringContent(object data)
        {
            StringContent stringContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");
            return stringContent;
        }

        public static StringContent ToTypeJSONStringContent<T>(T data)
        {
            StringContent stringContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize<T>(data),
                Encoding.UTF8,
                "application/json");
            return stringContent;
        }

        public static bool PropertyExist(dynamic dynamicObj, string property) {
            try {
                var value = dynamicObj[property].ToString();
                return true;
            }
            catch (Exception) {
                return false;
            }

        }

    }
}