using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.Web.Models.Cutter.CutterParameter;

namespace Wimi.BtlCore.Web.Controllers.Cutter
{
    public class CutterParameterController : BtlCoreControllerBase
    {
        private readonly ICutterAppService cutterAppService;

        public CutterParameterController(ICutterAppService cutterAppService)
        {
            this.cutterAppService = cutterAppService;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new CutterParameterModel();
            if (id.HasValue && id != 0)
            {
                model =
              ObjectMapper.Map<CutterParameterModel>(
                        await
                        this.cutterAppService.GetCutterParameterForEdit(new CutterParameterDto() { Id = (int)id }));
                model.IsEditMode = true;
            }

            return this.PartialView("~/Views/Cutter/CutterParameter/_CreateOrUpdateModal.cshtml", model);
        }

        // GET: ToolParameterMap
        public ActionResult Index()
        {
            return this.View("~/Views/Cutter/CutterParameter/Index.cshtml");
        }
    }
}
