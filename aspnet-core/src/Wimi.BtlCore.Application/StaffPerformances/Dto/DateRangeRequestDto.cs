namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;

    using Wimi.BtlCore.Extensions;

    public class DateRangeRequestDto
    {
        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateRangeType Type { get; set; }

        public DateTime? ToBegin()
        {
            if (!this.StartDate.HasValue)
            {
                return null;
            }

            switch (this.Type)
            {
                case DateRangeType.Null:
                    return this.StartDate.Value.DayDate();
                case DateRangeType.Shift:
                    return this.StartDate.Value.DayDate();
                case DateRangeType.Day:
                    return this.StartDate.Value.DayDate();
                case DateRangeType.Week:
                    return this.StartDate.Value.WeekEarly();
                case DateRangeType.Month:
                    return this.StartDate.Value.MonthEarly();
                case DateRangeType.Quarter:
                    return this.StartDate.Value.QuarterEarly();
                case DateRangeType.Year:
                    return this.StartDate.Value.YearEarly();
                default:
                    return this.StartDate.Value.DayDate();
            }
        }

        public DateTime? ToEnd()
        {
            if (!this.EndDate.HasValue)
            {
                return null;
            }

            switch (this.Type)
            {
                case DateRangeType.Null:
                    return this.EndDate.Value.TomorrowDayDate();
                case DateRangeType.Shift:
                    return this.EndDate.Value.TomorrowDayDate();
                case DateRangeType.Day:
                    return this.EndDate.Value.TomorrowDayDate();
                case DateRangeType.Week:
                    return this.EndDate.Value.NextWeekEarly();
                case DateRangeType.Month:
                    return this.EndDate.Value.NextMonthEarly();
                case DateRangeType.Quarter:
                    return this.EndDate.Value.NextQuarterEarly();
                case DateRangeType.Year:
                    return this.EndDate.Value.NextYearEarly();
                default:
                    return this.EndDate.Value.TomorrowDayDate();
            }
        }
    }
}