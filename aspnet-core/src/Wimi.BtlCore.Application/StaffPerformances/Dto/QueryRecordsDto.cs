namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;
    using System.Collections.Generic;

    using Wimi.BtlCore.Dto;

    /// <summary>
    /// 接受人员上下线日志的前台输入
    /// </summary>
    public class QueryRecordsDto : PagedSortedAndFilteredInputDto
    {
        public QueryRecordsDto()
        {
            this.MachineIdList = new List<int>();
        }

        public DateTime? EndTime { get; set; }

        public List<int> MachineIdList { get; set; }

        public DateTime? StartTime { get; set; }

        public string UserName { get; set; }
    }
}