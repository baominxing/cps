using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class StatusDistributionRequireDto
    {
        public StatusDistributionRequireDto()
        {
            this.MachineIdList = new List<long>();
        }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public List<long> MachineIdList { get; set; }

        public bool Feedback { get; set; }

        public EnumStatisticalMode Mode { get; set; }
        public List<string> UnionTables { get; set; }

    }
}