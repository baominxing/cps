using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Maintain.Dto;
using Wimi.BtlCore.Web.Models.Maintain.MaintainRepair;

namespace Wimi.BtlCore.Web.Controllers.Maintain
{
    public class MaintainRepairController : BtlCoreControllerBase
    {
        private IRepairRequestAppService repairRequestAppService;
        public MaintainRepairController(IRepairRequestAppService repairRequestAppService)
        {
            this.repairRequestAppService = repairRequestAppService;
        }
        // GET: Maintain
        public ActionResult Index()
        {
            return View("~/Views/Maintain/MaintainRepair/Index.cshtml");
        }
        public PartialViewResult LookOrRepaireRequest(RepairInputDto input)
        {
            var viewModel = new MaintainRepairViewModel();
            if (input.Id != 0)
            {
                var dto = repairRequestAppService.GetMaintainRepairList(new EntityDto() { Id = input.Id });
                viewModel = ObjectMapper.Map<MaintainRepairViewModel>(dto);
                viewModel.ShutDownStatus = dto.IsShutdown ? L("Yes") : L("No");
                viewModel.RequestPlanDate = dto.RequestDate.ToString("yyyy-MM-dd HH:mm:ss");
                viewModel.IsEditMode = input.IsEditMode;
                viewModel.Id = input.Id;
            }
            return this.PartialView("~/Views/Maintain/MaintainRepair/LookOrRepaireRequest.cshtml", viewModel);
        }


    }
}
