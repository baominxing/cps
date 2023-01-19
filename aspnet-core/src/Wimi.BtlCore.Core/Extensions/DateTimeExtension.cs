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
        /// ָ�����ڵĿ�ʼ���� 
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
        /// ָ�����ڵ������µĵ�һ�������
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
        /// ָ�����ڵ��¸����µĵ�һ�������
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextMonthEarly(this DateTime date)
        {
            var startMonth = date.MonthEarly();

            // �����³�
            var nextMonth = startMonth.AddMonths(1);
            return nextMonth.Date;
        }

        /// <summary>
        /// ָ�����ڵ��¼��ȵĵ�һ�������
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextQuarterEarly(this DateTime date)
        {
            // ���ȳ�
            var startQuarter = date.QuarterEarly();

            // �¼����ȳ�
            var nextQuarter = startQuarter.AddMonths(3);

            return nextQuarter.Date;
        }

        /// <summary>
        /// ָ�����������ܵ���������
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime NextWeekEarly(this DateTime date)
        {
            // ��һ
            var startWeek = date.WeekEarly();

            // ������һ
            var nextWeek = startWeek.AddDays(7);
            return nextWeek.Date;
        }

        /// <summary>
        /// ָ��������һ��ĵ�һ������
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
        /// ָ�����ڵ����ڼ��ȵĵ�һ�������
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
        /// ָ�����ڵ���������� 
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
        /// ָ�����������ܵĿ�ʼ����
        /// </summary>
        /// <param name="date">
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime WeekEarly(this DateTime date)
        {
            var wd = Convert.ToInt32(date.DayOfWeek.ToString("d"));

            // ��������� �������ת��
            if (wd == 0)
            {
                wd = 7;
            }

            var d = 1 - wd;

            // ��һ
            var startWeek = date.AddDays(d);

            return startWeek.Date;
        }

        /// <summary>
        /// ָ������������ݵĵ�һ������
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