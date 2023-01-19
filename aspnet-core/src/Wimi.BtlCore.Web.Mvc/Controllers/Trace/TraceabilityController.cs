using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Common.Modals;
using Wimi.BtlCore.Web.Models.Trace;

namespace Wimi.BtlCore.Web.Controllers.Trace
{
    public class TraceabilityController : BtlCoreControllerBase
    {
        private readonly IShiftSolutionManager shiftSolutionManager;

        public TraceabilityController(IShiftSolutionManager shiftSolutionManager)
        {
            this.shiftSolutionManager = shiftSolutionManager;
        }

        // GET: Tracking
        public async Task<ActionResult> Index(long? ngPartCatlogId)
        {
            var shiftSolution = await shiftSolutionManager.ListShiftSolution();
            var modal = new TraceabilitySelectGroupViewModal()
            {
                Items = shiftSolution.Select(n => new SelectGroupItemViewModal<ShiftSolutionItem>
                {
                    Name = n.Name,
                    ChildNode = n.Value
                })
            };
            ViewBag.NgPartCatlogId = ngPartCatlogId;
            return this.View("~/Views/Traceability/Query/Index.cshtml", modal);
        }

        public ActionResult Settings()
        {
            return this.View("~/Views/Traceability/Setting/Settings.cshtml");
        }

        public PartialViewResult ProcessParameters()
        {
            return this.PartialView("~/Views/Traceability/Query/_ProcessParameters.cshtml");
        }

        public PartialViewResult ExtensionData()
        {
            return this.PartialView("~/Views/Traceability/Query/_ExtensionData.cshtml");
        }

        public async Task<ActionResult> NgParts()
        {
            var shiftSolution = await shiftSolutionManager.ListShiftSolution();
            var modal = new TraceabilitySelectGroupViewModal()
            {
                Items = shiftSolution.Select(n => new SelectGroupItemViewModal<ShiftSolutionItem>
                {
                    Name = n.Name,
                    ChildNode = n.Value
                })
            };

            return this.View("~/Views/Traceability/NgParts/NgParts.cshtml", modal);
        }

        public PartialViewResult ShowDefectsModal()
        {
            return this.PartialView("~/Views/Traceability/NgParts/_ShowDefectsModal.cshtml");
        }

        public PartialViewResult CollectionDefectsModal()
        {
            return this.PartialView("~/Views/Traceability/NgParts/_CollectionDefectsModal.cshtml");
        }
    }


}