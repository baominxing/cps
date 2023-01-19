namespace Wimi.BtlCore.Web.Controllers.StaffPerformance
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.StaffPerformance;
    using Wimi.BtlCore.Web.Models.StatisticAnalysis.Alarm;

    public class StaffPerformanceController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public StaffPerformanceController(ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
        }

        /// <summary>
        /// 上下线
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> OnlineOrOffline()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new OnlineOrOfflineViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.View("OnlineOrOffline", model);
        }

        /// <summary>
        /// 上下线日志
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> OnlineOrOfflineRecord()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new OnlineOrOfflineRecordViewModel
            {
                DeviceGroups =
                                    deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.View(model);
        }

        public PartialViewResult OnlineShiftDetailModal()
        {
            return this.PartialView("_OnlineShiftDetailModal");
        }

        public PartialViewResult OnlineAllModal()
        {
            return this.PartialView("_OnlineAllModal");
        }

        public PartialViewResult OfflineDeviceGroupModal()
        {
            return this.PartialView("_OfflineDeviceGroupModal");
        }

        /// <summary>
        /// 员工绩效
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> StaffPerformance()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new StaffPerformanceViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };

            return this.View("StaffPerformance", model);
        }

        public async Task<PartialViewResult> FilterModal()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new AlarmStatisticsViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.PartialView("_FilterModal", modal);
        }

        public async Task<PartialViewResult> FilterModal2()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new AlarmStatisticsViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.PartialView("_FilterModal2", modal);
        }

        /// <summary>
        /// 员工产量
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> StaffYield()
        {
            return await Task.FromResult(this.View());
        }
    }
}