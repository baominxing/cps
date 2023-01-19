using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.WorkOrders;

namespace Wimi.BtlCore.Order.LoginReports.Dtos
{
    public class WorkOrdersRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string ProductionPlanCode { get; set; }

        public EnumWorkOrderState? State { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "ProductionPlanCode desc ,ProcessOrderSeq";
            }
        }
    }
}
