namespace Wimi.BtlCore.Web.Controllers.Reasons
{
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Controllers;

    public class ReasonFeedbackAnalysisController : BtlCoreControllerBase
    {
        // GET: ReasonFeedbackAnalysis
        public ActionResult Index()
        {
            return this.View("~/Views/Reasons/ReasonFeedbackAnalysis/Index.cshtml");
        }


        public PartialViewResult DetailModal()
        {
            return this.PartialView("~/Views/Reasons/ReasonFeedbackAnalysis/_DetailModal.cshtml");
        }
    }
}