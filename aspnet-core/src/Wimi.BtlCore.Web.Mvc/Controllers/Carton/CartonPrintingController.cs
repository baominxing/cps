using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Carton
{
    public class CartonPrintingController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return View("/Views/Carton/CartonPrinting/Index.cshtml");
        }

        public PartialViewResult SelectDefectsModal()
        {
            return this.PartialView("/Views/Carton/CartonPrinting/_SelectDefectsModal.cshtml");
        }
    }
}
