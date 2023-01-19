using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    public class ListProductionPlanDto : PagedSortedAndFilteredInputDto
    {
        public string PlanCode { get; set; }

        public string ProductCode { get; set; }
    }
}
