using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Collections.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.Product;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.Plan.ProcessPlans;
using Wimi.BtlCore.Plan.ProcessPlans.Dtos;
using Wimi.BtlCore.Web.Models.Plan.ProcessPlans;

namespace Wimi.BtlCore.Web.Controllers.Plan
{
    public class PlansController : BtlCoreControllerBase
    {
        private readonly IProductAppService productAppService;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IProcessPlanAppService processPlanAppService;

        public PlansController(IProductAppService productAppService, DeviceGroupManager deviceGroupManager, IProcessPlanAppService processPlanAppService)
        {
            this.productAppService = productAppService;
            this.deviceGroupManager = deviceGroupManager;
            this.processPlanAppService = processPlanAppService;
        }

        public IActionResult Index()
        {
            var model = new PlanViewModel();

            var typeofEnum = typeof(EnumPlanStatus);
            foreach (var enumValue in Enum.GetValues(typeofEnum))
            {
                var stateName = Enum.GetName(typeofEnum, enumValue);

                var comboboxItem = new ComboboxItemDto(enumValue.ToString(), stateName);
                if (stateName == EnumPlanStatus.InProgress.ToString()
                    || stateName == EnumPlanStatus.New.ToString())
                {
                    comboboxItem.IsSelected = true;
                }
                
                model.Status.Add(comboboxItem);
            }
            return View("/Views/Plan/ProcessPlans/Index.cshtml",model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Plan_Manage)]
        public async Task<PartialViewResult> CreateOrUpdatePlan(int? id)
        {
            var model = new PlanCreateOrEditViewModel();

            var planDto = await this.processPlanAppService.GetPlan(new NullableIdDto(id));
            var currentShift = await this.processPlanAppService.GetCurrentShiftInfo(planDto.DeviceGroupId);

            model.PlanDto = planDto;
            model.IsEditMode = planDto.Id > 0;
            if (!model.IsEditMode)
            {
                model.PlanDto.ShiftTarget = currentShift;
            }

            if (model.IsEditMode && model.PlanDto.ShiftTarget.IsNullOrEmpty())
            {
                model.PlanDto.ShiftTarget = currentShift;
            }
            model.ValidStatusList = GetValidStatusComboboxItemList(planDto);
            model.ValidProductList = GetValidProductComboboxItemList(planDto);
            model.ValidFirstClassDeviceGroupList = await ListFirstClassDeviceGroups(planDto);

            return this.PartialView("/Views/Plan/ProcessPlans/_CreateOrUpdatePlan.cshtml", model);
        }

        public async Task<PartialViewResult> State(int? id)
        {
            var model = new PlanCreateOrEditViewModel();
            var planDto = await this.processPlanAppService.GetPlan(new NullableIdDto(id));

            model.PlanDto = planDto;
            model.IsEditMode = planDto.Id > 0;
            model.ValidStatusList = GetValidStatusComboboxItemList(planDto);
            return this.PartialView("~/Views/Plan/ProcessPlans/_State.cshtml", model);
        }

        public async Task<PartialViewResult> ShiftAmount(int? id)
        {
            var model = new PlanCreateOrEditViewModel();
            var planDto = await this.processPlanAppService.GetPlan(new NullableIdDto(id));
            model.PlanDto = planDto;
            return this.PartialView("~/Views/Plan/ProcessPlans/_ShiftAmount.cshtml", model);
        }

        public PartialViewResult RelateMachines(int planId)
        {
            var model = new RelateMachinesModel(planId);
            return this.PartialView("~/Views/Plan/ProcessPlans/_RelateMachines.cshtml", model);
        }

        private List<ComboboxItemDto> GetValidStatusComboboxItemList(EditPlanDto planDto)
        {
            var newStatusComboxboxItem = new ComboboxItemDto(EnumPlanStatus.New.ToString(), L($"Plan-{EnumPlanStatus.New.ToString()}"));
            var inProgressStatusComboxboxItem = new ComboboxItemDto(EnumPlanStatus.InProgress.ToString(), L($"Plan-{EnumPlanStatus.InProgress.ToString()}"));
            var completeStatusComboxboxItem = new ComboboxItemDto(EnumPlanStatus.Complete.ToString(), L($"Plan-{EnumPlanStatus.Complete.ToString()}"));
            var pauseStatusComboxboxItem = new ComboboxItemDto(EnumPlanStatus.Pause.ToString(), L($"Plan-{EnumPlanStatus.Pause.ToString()}"));
            var validStatus = new List<ComboboxItemDto>();

            switch (planDto.Status)
            {
                case EnumPlanStatus.New:
                    {
                        validStatus.AddRange(new[] { inProgressStatusComboxboxItem });
                        break;
                    }
                case EnumPlanStatus.InProgress:
                    {
                        validStatus.AddRange(new[] { pauseStatusComboxboxItem, completeStatusComboxboxItem });
                        break;
                    }
                case EnumPlanStatus.Complete:
                    {
                        validStatus.AddRange(new[] { completeStatusComboxboxItem });
                        break;
                    }
                case EnumPlanStatus.Pause:
                    {
                        validStatus.AddRange(new[] { inProgressStatusComboxboxItem, completeStatusComboxboxItem });
                        break;
                    }
                case EnumPlanStatus.AutoComplete:
                    validStatus.AddRange(new[] { completeStatusComboxboxItem });
                    break;
            }

            return validStatus;
        }


        private List<ComboboxItemDto> GetValidProductComboboxItemList(EditPlanDto planDto)
        {
            var returnList = new List<ComboboxItemDto>();

            var productList = this.productAppService.ListProducts();

            foreach (var product in productList)
            {
                var currentProduct = new ComboboxItemDto(product.Id.ToString(), product.Name)
                {
                    IsSelected = product.Id == planDto.ProductId
                };

                returnList.Add(currentProduct);
            }

            return returnList;
        }



        private async Task<List<ComboboxItemDto>> ListFirstClassDeviceGroups(EditPlanDto planDto)
        {
            var returnList = new List<ComboboxItemDto>();
            var deviceGroups = await this.deviceGroupManager.ListFirstClassDeviceGroups();

            foreach (var item in deviceGroups)
            {
                var currentProduct = new ComboboxItemDto(item.Id.ToString(), item.DisplayName)
                {
                    IsSelected = item.Id == planDto.DeviceGroupId
                };

                returnList.Add(currentProduct);
            }

            return returnList;
        }
    }
}