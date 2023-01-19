using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Plan.ProcessPlans.Dtos
{

    [AutoMap(typeof(PlanTarget))]
    public class ShiftItemDto : EntityDto
    {
        public int SolutionId { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public int ShiftTargetAmount { get; set; }
    }
}
