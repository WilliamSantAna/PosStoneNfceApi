using System.Collections.Generic;
using System.Linq;

namespace PosStoneNfce.API.Portal.App.Common.Utils
{
    public static class Tools
    {
        public static Dictionary<string, object> ObjectToDictionary(object data)
        {
            var dataAsDictionary = data.GetType().GetProperties()
                                        .Select(pi => new { Value = pi.GetValue(data), Key = pi.Name })
                                        .Where(pi => pi.Value != null)
                                        .ToDictionary(pi => pi.Key, pi => pi.Value);

            return dataAsDictionary;
        }

    }
}