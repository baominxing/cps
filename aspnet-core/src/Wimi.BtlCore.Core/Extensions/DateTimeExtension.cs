using System.Runtime.CompilerServices;

namespace Wimi.BtlCore.Extensions
{
    using System;

    /// <summary>
    /// The date time extension.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 指定日期的开始日期 
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime DayDate(this DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// 指定日期的所在月的第一天的日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime MonthEarly(this DateTime date)
        {
            var startMonth = date.AddDays(1 - date.Day);
            return startMonth.Date;
        }

        /// <summary>
        /// 指定日期的下个月月的第一天的日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextMonthEarly(this DateTime date)
        {
            var startMonth = date.MonthEarly();

            // 下月月初
            var nextMonth = startMonth.AddMonths(1);
            return nextMonth.Date;
        }

        /// <summary>
        /// 指定日期的下季度的第一天的日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextQuarterEarly(this DateTime date)
        {
            // 季度初
            var startQuarter = date.QuarterEarly();

            // 下季季度初
            var nextQuarter = startQuarter.AddMonths(3);

            return nextQuarter.Date;
        }

        /// <summary>
        /// 指定日期所在周的下周日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextWeekEarly(this DateTime date)
        {
            // 周一
            var startWeek = date.WeekEarly();

            // 下周周一
            var nextWeek = startWeek.AddDays(7);
            return nextWeek.Date;
        }

        /// <summary>
        /// 指定日期下一年的第一天日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextYearEarly(this DateTime date)
        {
            var startYear = date.YearEarly();
            var nextYear = startYear.AddYears(1);
            return nextYear.Date;
        }

        /// <summary>
        /// 指定日期的所在季度的第一天的日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime QuarterEarly(this DateTime date)
        {
            var querterMonth = (date.Month - 1) % 3;
            var startQuarter = date.AddMonths(0 - querterMonth).AddDays(1 - date.Day);
            return startQuarter.Date;
        }

        /// <summary>
        /// 指定日期的明天的日期 
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime TomorrowDayDate(this DateTime date)
        {
            return date.AddDays(1).Date;
        }

        /// <summary>
        /// 指定日期所在周的开始日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime WeekEarly(this DateTime date)
        {
            var wd = Convert.ToInt32(date.DayOfWeek.ToString("d"));

            // 如果是周日 则需进行转换
            if (wd == 0)
            {
                wd = 7;
            }

            var d = 1 - wd;

            // 周一
            var startWeek = date.AddDays(d);

            return startWeek.Date;
        }

        /// <summary>
        /// 指定日期所在年份的第一天日期
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime YearEarly(this DateTime date)
        {
            var year = date.Year;
            return new DateTime(year, 1, 1).Date;
        }

        public static string ToMongoDateTime(this DateTime date)
        {
            return date.ToString("yyyyMMddHHmmssffff");
        }

        public static string ToLocalFormat(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}