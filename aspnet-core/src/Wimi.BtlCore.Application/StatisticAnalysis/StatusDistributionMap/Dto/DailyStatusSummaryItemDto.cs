using System;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class DailyStatusSummaryItemDto
    {
        public long MachineId { get; set; }

        public string StateCode { get; set; }

        public string StateName { get; set; }

        public string Hexcode { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime StartTime { get; set; }
    }
}