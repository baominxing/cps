using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Plan.ProcessPlans.Dtos;

namespace Wimi.BtlCore.Web.Models.Plan.ProcessPlans
{
    public class PlanCreateOrEditViewModel
    {
        public PlanCreateOrEditViewModel()
        {
           
        }

        public bool IsEditMode { get; set; }

        public EditPlanDto PlanDto { get; set; }

        public List<ComboboxItemDto> ValidStatusList { get; set; }

        public List<ComboboxItemDto> ValidProductList { get; set; }

        public List<ComboboxItemDto> ValidFirstClassDeviceGroupList { get; set; }

    }
}