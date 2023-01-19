using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using System.Threading.Tasks;

using Abp.MultiTenancy;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : BtlCoreControllerBase
    {
        public async Task<ActionResult> Index()
        {
            if (this.AbpSession.MultiTenancySide == MultiTenancySides.Tenant)
            {
                if (await this.IsGrantedAsync(PermissionNames.Pages_Tenant_Dashboard))
                {
                    return this.RedirectToAction("Index", "Dashboard");
                }
            }

            // Default page if no permission to the pages above
            return this.RedirectToAction("Index", "Welcome");
        }
    }
}
