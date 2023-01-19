using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class GetMachineStateRateInputDto : ISortedResultRequest, IShouldNormalize
    {
        public GetMachineStateRateInputDto()
        {
            this.MachineIdList = new List<int>();
            this.MachineShiftSolutionNameList = new List<string>();
        }

        // 时间区间
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        // 设备Id列表
        public List<int> MachineIdList { get; set; }

        //班次方案Id
        public List<int> ShiftSolutionIdList { get; set; } = new EditableList<int>();

        public List<string> MachineShiftSolutionNameList { get; set; }

        public string ProgramName { get; set; }

        // 查询方式
        public string QueryType { get; set; }

        public EnumQueryMethod QueryMethod { get; set; }

        public string Sorting { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }

        public string SummaryDate { get; set; }

        public int? TenantId { get; set; }

        public string CurrentShiftId { get; set; }

        public string CurrentShiftDay { get; set; }

        public List<string> UnionTables { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SummaryDate DESC";
            }
        }

    }
}
