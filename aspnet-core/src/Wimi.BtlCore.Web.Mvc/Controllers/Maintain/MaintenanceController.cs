using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.AppSystem.Caching;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Maintain;

namespace Wimi.BtlCore.Web.Controllers.Maintain
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Host_Maintenance)]
    public class MaintenanceController : BtlCoreControllerBase
    {
        private readonly ICachingAppService cachingAppService;

        public MaintenanceController(ICachingAppService cachingAppService)
        {
            this.cachingAppService = cachingAppService;
        }

        public ActionResult Index()
        {
            var allCaches = this.cachingAppService.GetAllCaches();
            var model = new MaintenanceViewModel { Caches = allCaches.Items };

            return this.View("~/Views/Maintain/Maintain/Index.cshtml", model);
        }
    }
}
