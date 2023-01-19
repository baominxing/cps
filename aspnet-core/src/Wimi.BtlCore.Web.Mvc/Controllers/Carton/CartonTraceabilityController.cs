using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Carton
{
    public class CartonTraceabilityController : BtlCoreControllerBase
    {
        // GET: CartonTraceability
        public ActionResult Index()
        {
            return View("/Views/Carton/CartonTraceability/Index.cshtml");
        }
    }
}
