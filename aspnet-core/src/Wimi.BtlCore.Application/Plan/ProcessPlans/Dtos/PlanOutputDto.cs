using Abp.Application.Services.Dto;
using System;

namespace Wimi.BtlCore.Plan.ProcessPlans.Dtos
{
    public class PlanOutputDto : EntityDto
    {
        public int PlanId { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public EnumPlanStatus PlanStatus { get; set; }
        public string ProductName { get; set; }
        public string MachineGroupName { get; set; }
        public int PlanAmount { get; set; }
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
