using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.MultiTenancy;
using Wimi.BtlCore.StatisticAnalysis.Alarms;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.Alarm;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    public class AlarmStatisticsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public AlarmStatisticsController(
            IAlarmsAppService alarmAppService, 
            IRepository<Tenant> tenantRepository, 
            ITenantAppService tenantAppService, 
            ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
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
            return this.PartialView("~/Views/StatisticAnalysis/AlarmStatistics/_FilterModal.cshtml", modal);
        }

        public async Task<ActionResult> Index(int? tenantId)
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
            return this.View("~/Views/StatisticAnalysis/AlarmStatistics/Index.cshtml", modal);
        }

        public PartialViewResult ShowAlarmsDetailModal()
        {
            return this.PartialView("~/Views/StatisticAnalysis/AlarmStatistics/_ShowAlarmsDetailModal.cshtml");
        }
    }
}