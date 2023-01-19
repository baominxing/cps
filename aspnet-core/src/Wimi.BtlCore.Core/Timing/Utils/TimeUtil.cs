using NodaTime;
using System;

namespace Wimi.BtlCore.Timing.Utils
{
    public class TimeUtil
    {
        public static DateTime GetCstDateTime()
        {
            Instant now = NodaTime.SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }
    }
}
