using System;

namespace PosStoneNfce.API.Portal.App.Common.Extensions
{
    public static class ByteArrayExtension
    {
        public static string ToBase64(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }
    }

    public static class StringExtension
    {
        public static string As(this string value, string alias)
        {
            return string.Concat(value, " AS ", alias);
        }
    }
}