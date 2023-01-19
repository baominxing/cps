using System.Collections.Generic;

using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    public class GetProductionPlanForEditDto
    {
        public IEnumerable<NameValueDto<int>> CarftList { get; set; }

        public ProductionPlanDto ProductionPlan { get; set; }

        public IEnumerable<ProductNameValueDto> ProductList { get; set; }
    }
}
