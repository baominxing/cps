namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.ShiftTargetYiled;
    using Wimi.BtlCore.Web.Models.BasicData.ShiftTargetYiled;

    public class ShiftTargetYiledController : BtlCoreControllerBase
    {
        private readonly IRepository<ShiftTargetYileds> shiftTargetYiledRepository;

        public ShiftTargetYiledController(IRepository<ShiftTargetYileds> shiftTargetYiledRepository)
        {
            this.shiftTargetYiledRepository = shiftTargetYiledRepository;
        }

        public PartialViewResult CreateShiftTargetYiledModal()
        {
            return this.PartialView("_CreateShiftTargetYiledModal");
        }

        public PartialViewResult EditTargetYiledModal(int id)
        {
            var targetYiled = this.shiftTargetYiledRepository.Get(id);
            var model = ObjectMapper.Map<ShiftTargetYiledViewModel>(targetYiled);
            return this.PartialView("_EditTargetYiledModal", model);
        }

        // GET: ShiftTargetYiled
        public ActionResult Index()
        {
            return this.View();
        }
    }
}