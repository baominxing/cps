using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    [AbpMvcAuthorize(PermissionNames.Pages_Order_MachineProcess)]
    public class MachineProcessController : BtlCoreControllerBase
    {
        // GET: MachineProcess
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/MachineProcess/Index.cshtml");
        }

        public PartialViewResult ChangeProductModal()
        {
            return this.PartialView("~/Views/Orders/MachineProcess/_ChangeProductModal.cshtml");
        }
    }
}