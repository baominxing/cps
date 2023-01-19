using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Maintain.Dto;
using Wimi.BtlCore.Web.Models.Maintain.MaintainOrder;

namespace Wimi.BtlCore.Web.Controllers.Maintain
{
    public class MaintainOrderController : BtlCoreControllerBase
    {
        private readonly IMaintainOrderAppService maintainOrderAppService;

        public MaintainOrderController(IMaintainOrderAppService maintainOrderAppService)
        {
            this.maintainOrderAppService = maintainOrderAppService;
        }

        // GET: Plans
        public ActionResult Index()
        {
            var model = new MaintainOrderViewModel();

            var typeofEnum = typeof(EnumMaintainOrderStatus);
            foreach (var enumValue in Enum.GetValues(typeofEnum))
            {
                var stateName = Enum.GetName(typeofEnum, enumValue);

                var comboboxItem = new ComboboxItemDto(enumValue.ToString(), stateName) { IsSelected = true };
                model.ValidStatusList.Add(comboboxItem);
            }

            return View("~/Views/Maintain/MaintainOrder/Index.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Maintain_Order)]
        public PartialViewResult CreateOrUpdateModal(int? id)
        {
            var model = new MaintainOrderViewModel();

            if (id.HasValue)
            {
                var maintainOrderDto = maintainOrderAppService.GetMaintainOrder(new MaintainOrderInputDto() { Id = id.Value });
                model.MaintainOrderDto = maintainOrderDto;
            }
            else
            {
                model.IsEditMode = false;
            }

            return this.PartialView("~/Views/Maintain/MaintainOrder/_CreateOrUpdateModal.cshtml", model);
        }
    }
}
