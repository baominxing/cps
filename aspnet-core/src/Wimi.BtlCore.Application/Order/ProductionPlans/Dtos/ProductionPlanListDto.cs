using System;
using System.Collections.Generic;

using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    [AutoMapFrom(typeof(ProductionPlan))]
    public class ProductionPlanListDto : ProductionPlanDto
    {
        public DateTime? ActualEndDate { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public bool CanDelete { get; set; }

        public int DefectiveCount { get; set; }

        public string DisplayState { get; set; }

        public int QualifiedCount { get; set; }

        public virtual IEnumerable<WorkOrderDto> WorkOrders { get; set; }
    }
}
