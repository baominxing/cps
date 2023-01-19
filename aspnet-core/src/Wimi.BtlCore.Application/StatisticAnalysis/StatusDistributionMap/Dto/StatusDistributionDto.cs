using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class StatusDistributionDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public DateTime ShiftDay { get; set; }

        public IEnumerable<StatusDistributionItemDto> Data { get ; set; }

        public IEnumerable<DailyStatusSummaryDto> StatusSummaryRate { get; set; }
    }
}