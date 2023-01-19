using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    public class DefectiveStatisticsController : BtlCoreControllerBase
    {
        // GET: DefectiveStatistics
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/DefectiveStatistics/Index.cshtml");
        }
    }
}