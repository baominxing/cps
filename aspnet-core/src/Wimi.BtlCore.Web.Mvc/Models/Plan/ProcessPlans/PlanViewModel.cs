using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Web.Models.Plan.ProcessPlans
{
    public class PlanViewModel
    {
        public PlanViewModel()
        {
            this.Status = new List<ComboboxItemDto>();
        }
        public List<ComboboxItemDto> Status { get; set; }
    }
}