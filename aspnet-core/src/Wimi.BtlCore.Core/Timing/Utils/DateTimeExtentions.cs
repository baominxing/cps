using System;

namespace Wimi.BtlCore.Timing.Utils
{
    public static class DateTimeExtentions
    {
        public static DateTime ToCstTime(this DateTime time)
        {
            return TimeUtil.GetCstDateTime();
        }

        public static DateTime ConvertTodayToCstTime(this DateTime time)
        {
            return TimeUtil.GetCstDateTime().Date;
        }
    }
}
