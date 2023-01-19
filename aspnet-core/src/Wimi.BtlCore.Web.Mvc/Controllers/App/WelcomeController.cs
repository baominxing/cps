using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers
{
    [AbpMvcAuthorize]
    public class WelcomeController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}