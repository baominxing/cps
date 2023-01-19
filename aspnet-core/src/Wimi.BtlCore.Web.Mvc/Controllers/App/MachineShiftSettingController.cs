namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Shifts;
    using Wimi.BtlCore.Shifts.Dto;
    using Wimi.BtlCore.Web.Models.BasicData.MachineSetting;
    using Wimi.BtlCore.Web.Models.StatisticAnalysis.EfficiencyTrend;

    public class MachineShiftSettingController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IShiftAppService shiftAppService;

        public MachineShiftSettingController(
            ICommonLookupAppService commonLookupAppService, 
            IShiftAppService shiftAppService)
        {
            this.shiftAppService = shiftAppService;
            this.commonLookupAppService = commonLookupAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_MachineShiftSetting_Manage)]
        public async Task<PartialViewResult> CreateDeviceGroupShiftSolutionModal()
        {
            var deviceGroupAndMachineWithPermissions =
                   await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new MultiMachineShiftModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.PartialView("Views/App/MachineShiftSetting/CreateDeviceGroupShiftSolutionModal.cshtml", modal);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_MachineShiftSetting_Manage)]
        public async Task<PartialViewResult> CreateDeviceShiftSolutionModal(int id)
        {
            var result =
                await this.shiftAppService.GetMachineShiftSolution(new Machine4ShiftSolutionInputDto() { Id = id });
            var model = new MachineModel
            {
                Id = result.MachineId,
                MachineShiftSolution = (from e in result.Machine4ShiftSolutionDetail
                    select
                        new MachineShiftSolutionViewModel()
                        {
                            Id = e.Id,
                            ShiftSolutionId = e.ShiftSolutionId,
                            Name = e.Name,
                            StartTime = e.StartTime,
                            EndTime = e.EndTime,
                            IsInUse = DateTime.Today>= e.StartTime && DateTime.Today<= e.EndTime
                        }).ToList()
            };
            return this.PartialView("Views/App/MachineShiftSetting/CreateDeviceShiftSolutionModal.cshtml", model);
        }

        // GET: MachineShiftSetting
        public async Task<ActionResult> Index()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new EfficiencyTrendsViewModel
                            {
                                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups, 
                                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds, 
                                Machines = deviceGroupAndMachineWithPermissions.Machines.Where(m=>m.IsActive==true).ToList()
                            };

            return this.View("Views/App/MachineShiftSetting/Index.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_MachineShiftSetting_Manage)]
        public async Task<PartialViewResult> ViewDeviceShiftSolutionHistoryModal(int id)
        {
            var model =
                await
                this.shiftAppService.GetDeviceHistoryShiftInfo(
                    new DeviceHistoryShiftInfoInputDto() { DeviceId = id });
            return this.PartialView("Views/App/MachineShiftSetting/ViewDeviceShiftSolutionHistoryModal.cshtml", model);
        }
    }
}