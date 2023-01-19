using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    public class ProductionPlanController : BtlCoreControllerBase
    {
        public PartialViewResult CreateOrEditModal()
        {
            return this.PartialView("~/Views/Orders/ProductionPlan/_CreateOrEditModal.cshtml");
        }

        // GET: ProductionPlan
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/ProductionPlan/Index.cshtml");
        }
    }
}