using System;
using Wimi.BtlCore.Plan;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class ProcessPlanResultDto
    {
        public string PlanCode { get; set; }
        public string PlanName { get; set; }

        public string ProductName { get; set; }

        public int PlanAmount { get; set; }

        public int CompleteAmount { get; set; }

        public string TotalCompleteRate { get; set; }

        public string StatisticalWay { get; set; }

        public int StatisticalWayAmount { get; set; }

        public string SummaryDate { get; set; }

        public int SummaryDateAmount { get; set; }

        public string SummaryDateCompleteRate { get; set; }

        public int PlanId { get; set; }
        public EnumPlanStatus PlanStatus { get; set; }
        public string MachineGroupName { get; set; }
        public EnumTargetDimension TargetType { get; set; }
        public EnumYieldSummaryType YieldSummaryType { get; set; }
        public DateTime? RealStartTime { get; set; }
        public DateTime? RealEndTime { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public int TargetAmount { get; set; }
        public int ProcessAmount { get; set; }
        public int ShiftTargetAmount { get; set; }
        public string ShiftName { get; set; }
    }
}
