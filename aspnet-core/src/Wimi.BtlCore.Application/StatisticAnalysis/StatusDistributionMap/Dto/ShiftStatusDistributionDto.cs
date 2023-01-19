using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class ShiftStatusDistributionDto
    {
        public string ShiftName { get; set; }

        public IEnumerable<StatusDistributionItemDto> Data { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public long MachineId { get; set; }

        public int? MachineShiftDetailId { get; set; }

        public IEnumerable<DailyStatusSummaryDto> StatusSummaryRate { get; set; }
    }
}