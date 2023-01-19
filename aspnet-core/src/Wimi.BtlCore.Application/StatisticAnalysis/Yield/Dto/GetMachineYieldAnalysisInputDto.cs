using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    public class GetMachineYieldAnalysisInputDto : ISortedResultRequest, IShouldNormalize
    {
        public string EndTime { get; set; }

        // 设备Id列表
        public List<int> IdList { get; set; }

        public string ProgramName { get; set; }

        // 查询方式
        public EnumQueryMethod QueryType { get; set; }

        public string Sorting { get; set; }

        // 时间区间
        public string StartTime { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }

        public string SummaryDate { get; set; }

        public int? TenantId { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SummaryDate DESC";
            }
        }
    }
}
