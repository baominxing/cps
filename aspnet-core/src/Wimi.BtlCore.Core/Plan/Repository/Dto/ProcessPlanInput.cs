using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Plan.Repository.Dto
{
    public class ProcessPlanInput
    {
        public DateTime EndTime { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 目标维度
        /// </summary>
        public EnumTargetDimension TargetType { get; set; }

        public int PlanId { get; set; }

        //产量计算方式
        public EnumYieldSummaryType YieldSummaryType { get; set; }

        public int MachineId { get; set; }

        public DateTime? RealStartTime { get; set; }

        public int DeviceGroupId { get; set; }
        public List<string> UnionTables { get; set; } = new List<string>();
    }
}
