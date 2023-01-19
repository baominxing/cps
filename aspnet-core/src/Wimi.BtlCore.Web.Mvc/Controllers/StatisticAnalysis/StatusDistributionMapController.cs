using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    public class StatusDistributionMapController : BtlCoreControllerBase
    {
        // GET: StatusDistributionMap
        public ActionResult Index()
        {
            return View("~/Views/StatisticAnalysis/StatusDistributionMap/Index.cshtml");
        }

        public PartialViewResult ShowStateDetailModal()
        {
            return PartialView("~/Views/StatisticAnalysis/StatusDistributionMap/_ShowStateDetailModal.cshtml");
        }
    }
}