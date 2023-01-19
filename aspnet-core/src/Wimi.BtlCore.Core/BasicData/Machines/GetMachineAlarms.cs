namespace Wimi.BtlCore.BasicData.Machines
{
    using System;
    using System.Collections.Generic;

    public class GetMachineAlarms
    {
        public GetMachineAlarms()
        {
            this.MachineIdList = new List<int>();
            this.MachineShiftSolutionNameList = new List<string>();
        }

        public string AlarmCode { get; set; }

        public DateTime? EndTime { get; set; }

        // 设备Id列表
        public List<int> MachineIdList { get; set; }

        public List<int> MachineShiftSolutionIdList { get; set; }

        public List<string> MachineShiftSolutionNameList { get; set; }

        public string QueryType { get; set; }

        public string SelectString { get; set; }

        // 时间区间
        public DateTime? StartTime { get; set; }

        public string StatisticalWay { get; set; }

        public string SummaryDate { get; set; }

        public int? TenantId { get; set; }

        public DateTime ShiftDay { get; set; }

        public int ShiftItemSeq { get; set; }
        public List<string> UnionTables { get; set; }
    }
}