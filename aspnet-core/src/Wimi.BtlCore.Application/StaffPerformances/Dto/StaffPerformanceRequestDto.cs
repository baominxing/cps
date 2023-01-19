namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.AutoMapper;

    using Wimi.BtlCore.StaffPerformance;

    [AutoMap(typeof(StaffPerformance))]
    public class StaffPerformanceRequestDto
    {
        public StaffPerformanceRequestDto()
        {
            this.MachineIdList = new List<int>();
            this.UserIdList = new List<long>();
            this.UnionTables = new List<string>();
        }

        public DateTime? EndTime { get; set; }

        // 设备Id列表
        public List<int> MachineIdList { get; set; }

        // 0:按人员查询,1:按设备查询
        public EnumStaffPerformanceQueryType QueryType { get; set; }

        // 时间区间
        public DateTime? StartTime { get; set; }

        public string StatisticalWay { get; set; }

        // 用户Id列表
        public List<long> UserIdList { get; set; }

        public int ShiftSolutionId { get; set; }

        public List<string> UnionTables { get; set; }
    }
}