using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    public class GetMachineYieldInputDto : ISortedResultRequest, IShouldNormalize
    {
        public GetMachineYieldInputDto()
        {
            ShiftSolutionIdList = new List<int>();
        }

        public string EndTime { get; set; }

        // 设备Id列表
        public List<int> MachineIdList { get; set; }

        //班次方案Id
        public List<int> ShiftSolutionIdList { get; set; }

        public List<int> DeviceGroupIdList { get; set; }

        // 查询方式
        public string QueryType { get; set; }

        public EnumQueryMethod QueryMethod { get; set; }

        public string Sorting { get; set; }

        // 时间区间
        public string StartTime { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }

        public string SummaryDate { get; set; }

        public int? TenantId { get; set; }

        public List<string> UnionTables { get; internal set; } = new List<string>();

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SummaryDate DESC";
            }
        }
    }
}
