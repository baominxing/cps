using System;
using System.Globalization;

namespace Wimi.BtlCore.Extensions
{
    public static class StringExtension
    {
        public static DateTime MongoDateTimeParseExact(this string dateString)
        {
            return DateTime.ParseExact(dateString, dateString.Length == 18 ? "yyyyMMddHHmmssffff" : "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
        }


        public static DateTime DateTimeParseExact14(this string dateString)
        {
            return DateTime.ParseExact(dateString, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
        }

        public static string FormartMongoDateTime(this string dateTime)
        {
            var date = dateTime.MongoDateTimeParseExact();
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string FormartMongoDateTimeWithMillisecond(this string dateTime)
        {
            var date = dateTime.MongoDateTimeParseExact();
            return date.ToString("yyyy-MM-dd HH:mm:ss:ffff");
        }
    }
}