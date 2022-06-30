using System;
using System.Globalization;

namespace PosStoneNfce.API.Portal.App.Common.Utils
{
    public static class DateExtensions
    {
        public static int GetEndOfDayTimestamp() 
        {
            DateTime date = DateTime.Now;
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            DateTime endOfDay = new DateTime(year, month, day, 23, 59, 59);
            string unixTimestamp = Convert.ToString((int) endOfDay.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            return Int32.Parse(unixTimestamp);
        
        }

        public static bool DateTimestampIsExpired(int timestamp) {
            int timestampNowInt = GetTimestampNow();
            return (timestampNowInt < timestamp ? false : true);
        }

        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp ) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }

        public static int DateToTimestamp(string datetime) {
            DateTime oDate = Convert.ToDateTime(datetime);
            int timestamp = Int32.Parse(Convert.ToString((int) oDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
            return timestamp;
        }

        public static int GetTimestampNow() {
            DateTime date = DateTime.Now;
            int timestampNow = Int32.Parse(Convert.ToString((int) date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
            return timestampNow;            
        }
    }
}