using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.EfficiencyTrendas.Dtos
{
    public class EfficiencyTrendsInputDto
    {

        public EfficiencyTrendsInputDto()
        {
            this.MachineShiftSolutionNameList = new List<string>();
        }

        public string ShiftName { get; set; }

        public DateTime EndTime { get; set; }

        public string GroupType { get; set; }

        public List<int> MachineId { get; set; }

        public string MachineName { get; set; }

        public List<int> MachineShiftDetailId { get; set; }

        /// <summary>
        /// 查询方式：1=设备组，0=设备
        /// </summary>
        public string QueryType { get; set; }

        public DateTime StartTime { get; set; }

        public string StatisticalWays { get; set; }

        public List<string> MachineShiftSolutionNameList { get; set; }

        public List<string> UnionTables { get; set; }
    }

}
