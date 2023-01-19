using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Web.Models.Maintain.MaintainRequest;

namespace Wimi.BtlCore.Web.Controllers.Maintain
{
    public class MaintainRequestController : BtlCoreControllerBase
    {
        private IRepairRequestAppService repairRequestAppService;
        public MaintainRequestController(IRepairRequestAppService repairRequestAppService)
        {
            this.repairRequestAppService = repairRequestAppService;
        }
        // GET: MaintianRequest
        public ActionResult Index()
        {
            return View("~/Views/Maintain/MaintainRequest/Index.cshtml");
        }
        public PartialViewResult CreateOrUpdateRepaireRequest(int id)
        {
            var viewModel = new MaintainRequestViewModel();
            if (id != 0)
            {
                var dto = repairRequestAppService.GetMaintainRequest(new EntityDto() { Id = id });
               //dto.MapTo(viewModel);
                ObjectMapper.Map(dto, viewModel);
                viewModel.IsEditMode = true;

            }
            return this.PartialView("~/Views/Maintain/MaintainRequest/_CreateOrUpdateRepaireRequest.cshtml", viewModel);
        }
        public PartialViewResult LookRepaireRequest(int id)
        {
            var viewModel = new MaintainRequestViewModel();
            if (id != 0)
            {
                var dto = repairRequestAppService.GetMaintainRequestOnLook(new EntityDto() { Id = id });
                //dto.MapTo(viewModel);
                ObjectMapper.Map(dto, viewModel);
                viewModel.IsEditMode = true;

            }
            return this.PartialView("~/Views/Maintain/MaintainRequest/_LookRepairRequest.cshtml", viewModel);
        }
    }
}
