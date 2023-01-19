namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.AutoMapper;
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Dto;

    [AutoMap(typeof(GetMachineAlarms))]
    public class GetMachineAlarmsInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public GetMachineAlarmsInputDto()
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

        public string WhereClause { get; set; }

        public List<string> UnionTables { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "StartTime,Code";
            }
        }
    }
}