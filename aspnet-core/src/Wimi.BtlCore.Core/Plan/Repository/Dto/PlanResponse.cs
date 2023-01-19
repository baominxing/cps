using System;

namespace Wimi.BtlCore.Plan.Repository.Dto
{
    public class PlanResponse
    {
        public string StatisticalWay { get; set; }

        public decimal StatisticalWayAmount { get; set; }

        public int Id { get; set; }

        public string PlanCode { get; set; }
        public string PlanName { get; set; }

        public string ProductName { get; set; }

        public int PlanAmount { get; set; }

        public int DeviceGroupId { get; set; }

        public EnumTargetDimension TargetType { get; set; }

        public int TargetAmount { get; set; }

        public EnumYieldSummaryType YieldSummaryType { get; set; }

        public int YieldCounterMachineId { get; set; }

        public int ProcessAmount { get; set; }

        public DateTime RealStartTime { get; set; }

        public DateTime RealEndTime { get; set; }
    }
}
