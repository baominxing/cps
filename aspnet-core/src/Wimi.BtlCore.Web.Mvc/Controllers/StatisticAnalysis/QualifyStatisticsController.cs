using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    [AbpAuthorize(PermissionNames.Pages_QualifyStatistics)]
    public class QualifyStatisticsController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return View("~/Views/StatisticAnalysis/QualifyStatistics/Index.cshtml");
        }
    }
}

