using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.StatisticAnalysis.States;
using Wimi.BtlCore.ThirdpartyApis.Dto;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.EfficiencyTrend;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.TimeStatistics;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    public class TimeStatisticsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IStatesAppService stateAppService;

        public TimeStatisticsController(
            IStatesAppService stateAppService, 
            ICommonLookupAppService commonLookupAppService)
        {
            this.stateAppService = stateAppService;
            this.commonLookupAppService = commonLookupAppService;
        }

        [HttpPost]
        public async Task<JsonResult> GetMachineStateRateByMac(GetMachineStateRateInputDto input)
        {
            var json = await this.stateAppService.GetMachineStateRateByMac(input);
            return this.Json(json);
        }

        public async Task<ActionResult> Index(int? tenantId)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new EfficiencyTrendsViewModel
                            {
                                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups, 
                                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds, 
                                Machines = deviceGroupAndMachineWithPermissions.Machines
                            };

            return this.View("~/Views/StatisticAnalysis/TimeStatistics/Index.cshtml", model);
        }

        public async Task<PartialViewResult> FilterModal()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new TimeStatisticsViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.PartialView("~/Views/StatisticAnalysis/TimeStatistics/_FilterModal.cshtml", modal);
        }

    }
}