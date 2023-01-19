using Abp.Application.Services.Dto;
using System.Collections.Generic;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Web.Models.Maintain.MaintainPlan
{
    public class MaintainPlanViewModel
    {
        public bool IsEditMode { get; set; } = true;

        public MaintainPlanDto MaintainPlanDto { get; set; } = new MaintainPlanDto();

        public List<ComboboxItemDto> ValidStatusList { get; set; } = new List<ComboboxItemDto>();
    }
}
