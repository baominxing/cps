using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Plan.ProcessPlans.Dtos
{
    public class PlanInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }
        public List<EnumPlanStatus> QueryStatus { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeEnd { get; set; }

        public DateTime? InitialStartTime { get; set; }

        public DateTime? InitialEndTime { get; set; }

        public string PlanName { get; set; }

        public string ProductName { get; set; }

        public PlanInputDto()
        {
            QueryStatus = new List<EnumPlanStatus>();
        }

        public void Normalize()
        {
            if (this.Sorting.IsNullOrWhiteSpace())
            {
                this.Sorting = "PlanStartTime ";
            }
        }
    }
}
