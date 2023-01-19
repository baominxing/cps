namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.Runtime.Validation;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Dto;

    public class GetMachineAlarmStatisticesInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public GetMachineAlarmStatisticesInput()
        {
            this.MachineIdList = new List<long>();
        }

        public string AlarmCode { get; set; }

        public DateTime? DateTimeEnd { get; set; }

        // 时间区间
        public DateTime? DateTimeFrom { get; set; }

        // 设备Id列表
        public List<long> MachineIdList { get; set; }

        public string QueryType { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }

        public int? TenantId { get; set; }

        public void Normalize()
        {
            this.Sorting = "StatisticalWayString DESC";
        }
    }
}