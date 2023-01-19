using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.CraftMaintain;
using Wimi.BtlCore.Web.Models.CraftMaintain;

namespace Wimi.BtlCore.Web.Controllers.CraftMaintain
{
    [AbpMvcAuthorize(PermissionNames.Pages_CraftMaintain_FlexibleCraftPath)]
    public class FlexibleCraftPathController : BtlCoreControllerBase
    {
        private readonly IFlexibleCraftMaintainAppService flexibleCraftMaintainAppService;
        public FlexibleCraftPathController(IFlexibleCraftMaintainAppService flexibleCraftMaintainAppService)
        {
            this.flexibleCraftMaintainAppService = flexibleCraftMaintainAppService;
        }
        public ActionResult Index()
        {
            return View("~/Views/CraftMaintain/FlexibleCraftPath/Index.cshtml");
        }

        public ActionResult CreateProcesseModal()
        {
            return PartialView("~/Views/CraftMaintain/FlexibleCraftPath/_CreateProcesseModal.cshtml");
        }

        public ActionResult CraftDrawerModal(CraftDrawerModel model)
        {
            return PartialView("~/Views/CraftMaintain/FlexibleCraftPath/_CraftDrawerModal.cshtml", model);
        } 

        public ActionResult CraftPathMap(int id)
        {
            return View("~/Views/CraftMaintain/FlexibleCraftPath/_CraftPathMap.cshtml");
        }
    }
}