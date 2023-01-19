using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
