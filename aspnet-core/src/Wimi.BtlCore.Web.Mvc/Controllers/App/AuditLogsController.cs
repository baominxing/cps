using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.App
{
    [DisableAuditing]
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_AuditLogs)]
    public class AuditLogsController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}
