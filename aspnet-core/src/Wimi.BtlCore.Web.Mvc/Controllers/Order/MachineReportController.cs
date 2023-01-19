using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    [AbpMvcAuthorize(PermissionNames.Pages_Order_MachineReport)]
    public class MachineReportController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/MachineReport/index.cshtml");
        }

        public PartialViewResult FeedbackDefectiveReason()
        {
            return this.PartialView("~/Views/Orders/MachineReport/_FeedbackDefectiveReason.cshtml");
        }
    }
}