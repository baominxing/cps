namespace Wimi.BtlCore.StaffPerformances
{
    using System;
    using System.Linq.Expressions;

    using Abp.Specifications;
    using Wimi.BtlCore.BasicData.Capacities;

    public class CapacityDateRangeSpec : Specification<Capacity>
    {
        public CapacityDateRangeSpec(DateTime? start, DateTime? end)
        {
            this.Start = start;
            this.End = end;
        }

        public DateTime? End { get; private set; }

        public DateTime? Start { get; private set; }

        public override Expression<Func<Capacity, bool>> ToExpression()
        {
            if (!(this.Start.HasValue && this.End.HasValue))
            {
                return c => true;
            }

            var start = this.Start.Value.Date;

            var end = this.End.Value.Date;

            return c => c.ShiftDetail.ShiftDay.Value >= start && c.ShiftDetail.ShiftDay.Value < end;
        }
    }
}