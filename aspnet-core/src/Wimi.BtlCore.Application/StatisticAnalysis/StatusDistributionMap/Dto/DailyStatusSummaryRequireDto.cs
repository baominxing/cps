using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class DailyStatusSummaryRequireDto
    {
        public DailyStatusSummaryRequireDto()
        {
            UnionTables = new List<string>();
        }

        public DailyStatusSummaryRequireDto(int machineId, int? machinesShiftDetailId)
        {
            this.MachineId = machineId;
            this.MachinesShiftDetailId = machinesShiftDetailId;
        }

        public DailyStatusSummaryRequireDto(int machineId, DateTime shiftDay)
        {
            this.MachineId = machineId;
            this.MachinesShiftDetailId = null;
            this.ShiftDay = shiftDay;
        }

        public int MachineId { get; set; }

        public int? MachinesShiftDetailId { get; set; }

        public DateTime ShiftDay { get; set; }
        public List<string> UnionTables { get; set; }
    }
}