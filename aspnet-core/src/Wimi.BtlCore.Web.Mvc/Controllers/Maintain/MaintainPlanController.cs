using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Maintain.Dto;
using Wimi.BtlCore.Web.Models.Maintain.MaintainPlan;

namespace Wimi.BtlCore.Web.Controllers.Maintain
{
    public class MaintainPlanController : BtlCoreControllerBase
    {
        private readonly IMaintainPlanAppService maintainPlanAppService;

        public MaintainPlanController(IMaintainPlanAppService maintainPlanAppService)
        {
            this.maintainPlanAppService = maintainPlanAppService;
        }

        // GET: Plans
        public ActionResult Index()
        {
            var model = new MaintainPlanViewModel();

            var typeofEnum = typeof(EnumMaintainPlanStatus);
            foreach (var enumValue in Enum.GetValues(typeofEnum))
            {
                var stateName = Enum.GetName(typeofEnum, enumValue);

                var comboboxItem = new ComboboxItemDto(enumValue.ToString(), stateName);
                comboboxItem.IsSelected = true;
                model.ValidStatusList.Add(comboboxItem);
            }

            return View("~/Views/Maintain/MaintainPlan/Index.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Maintain_Plan)]
        public PartialViewResult CreateOrUpdateModal(int? id)
        {
            var model = new MaintainPlanViewModel();

            if (id.HasValue)
            {
                var maintainPlanDto = this.maintainPlanAppService.GetMaintainPlan(new MaintainPlanInputDto { Id = (int)id });
                model.MaintainPlanDto = maintainPlanDto;
            }
            else
            {
                model.IsEditMode = false;
            }

            return this.PartialView("~/Views/Maintain/MaintainPlan/_CreateOrUpdateModal.cshtml", model);
        }
    }
}
