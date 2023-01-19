namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using System;

    using Abp.AutoMapper;
    using Wimi.BtlCore.OEE;

    [AutoMap(typeof(OeeResponse))]
    public class MachinePerformanceIndicator
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int MachineId { get; set; }

        public decimal PerfectTime { get; set; }

        public decimal ActualTime => this.TotalYiled == 0 ? 1 : Math.Round(this.TotalDuration / this.TotalYiled, 2);

        public decimal TotalDuration { get; set; }

        public decimal TotalYiled { get; set; }

        public string ShiftDay { get; set; }

        public decimal Rate => this.ActualTime == 0 || this.PerfectTime == 0
                                   ? 1
                                   : Math.Round(this.PerfectTime / this.ActualTime, 4);
    }
}