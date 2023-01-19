using Abp.Domain.Repositories;
using System;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.Shifts;
using Wimi.BtlCore.Shifts.Dto;
using Wimi.BtlCore.Web.Models.BasicData.Shifts;

namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    public class ShiftSettingController : BtlCoreControllerBase
    {
        private readonly IShiftAppService shiftAppService;
        private readonly IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;

        public ShiftSettingController(IShiftAppService shiftAppService, IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveRepository, IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
        {
            this.shiftAppService = shiftAppService;
            this.machineShiftEffectiveRepository = machineShiftEffectiveRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_ShiftSetting_Manage)]
        public async Task<PartialViewResult> CreateShiftInfoModal(int id)
        {
            var model = new CreateShiftInfoModalViewModel
            {
                ShiftSolutionId = id,
                CreateShiftInfoModalDetailViewModel = (from item in
                        await this.shiftAppService.GetShiftInfosForModal(new ShiftSolutionInputDto() {Id = id})
                    select
                        new CreateShiftInfoModalDetailViewModel()
                        {
                            ShiftSolutionId = item.ShiftSolutionId,
                            Id = item.Id,
                            Name = item.Name,
                            StartTime = item.StartTime,
                            EndTime = item.EndTime,
                            Duration = item.Duration,
                            CreationTime = item.CreationTime,
                            IsNextDay = item.IsNextDay
                        }).ToArray()
            };

            model.State = model.CreateShiftInfoModalDetailViewModel.Length == 0 ? EnumOperationState.Create : EnumOperationState.Edit;

            model.IsUsing = await this.machineShiftEffectiveRepository.GetAll().AnyAsync(m =>
                m.ShiftSolutionId == id && m.StartTime <= DateTime.Today && m.EndTime >= DateTime.Today);

            model.CanDelete =!this.shiftDayTimeRangeRepository.ListMachineShiftEffectiveIntervalTimeRange(id).Any(s=>s.StartTime<=DateTime.Now);
            return this.PartialView("Views/App/ShiftSetting/_CreateShiftInfoModal.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_ShiftSetting_Manage)]
        public async Task<PartialViewResult> CreateShiftSolutionModal(int? id)
        {
            var model = new CreateShiftSolutionModalViewModel();
            if (id == null)
            {
                model.State = EnumOperationState.Create;
            }
            else
            {
                var ss = await this.shiftAppService.GetShiftSolutionForModal(new ShiftSolutionInputDto() { Id = (int)id });
                if (ss != null)
                {
                    model.Id = ss.Id;
                    model.Name = ss.Name;
                    model.State = EnumOperationState.Edit;
                }
            }

            return this.PartialView("Views/App/ShiftSetting/_CreateShiftSolutionModal.cshtml", model);
        }

        // GET: ShiftSetting
        public ActionResult Index()
        {
            return this.View("Views/App/ShiftSetting/Index.cshtml");
        }
    }
}