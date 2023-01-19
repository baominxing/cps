namespace Wimi.BtlCore.StaffPerformance
{
    using System;
    using System.Collections.Generic;

    public class StaffPerformance
    {
        public StaffPerformance()
        {
            this.MachineIdList = new List<int>();
            this.UserIdList = new List<long>();
            this.UnionTables = new List<string>();
        }

        public DateTime? EndTime { get; set; }

        public int MachineId { get; set; }

        // 设备Id列表
        public List<int> MachineIdList { get; set; }

        public string MachineName { get; set; }

        // 0:按人员查询,1:按设备查询
        public EnumStaffPerformanceQueryType QueryType { get; set; }

        // 时间区间
        public DateTime? StartTime { get; set; }

        public string StatisticalWay { get; set; }

        public long UserId { get; set; }

        // 用户Id列表
        public List<long> UserIdList { get; set; }

        public string UserName { get; set; }

        public int ShiftSolutionId { get; set; }
        public List<string> UnionTables { get; set; }

        public bool UserIdListAllSelected()
        {
            // 包含0,说明在前台有选择全部这个选项
            return this.UserIdList.Contains(0);
        }
    }
}