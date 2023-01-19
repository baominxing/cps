using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Carton
{
    public class CartonSettingController : BtlCoreControllerBase
    {
        // GET: CartonSetting
        public ActionResult Index()
        {
            return View("/Views/Carton/CartonSetting/Index.cshtml");
        }
    }
}
