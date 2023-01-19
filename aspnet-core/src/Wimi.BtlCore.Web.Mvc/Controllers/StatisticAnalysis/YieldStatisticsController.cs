namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.StatisticAnalysis.Yield;
    using Wimi.BtlCore.StatisticAnalysis.Yield.Dto;
    using Wimi.BtlCore.Web.Models.StatisticAnalysis.Alarm;
    using Wimi.BtlCore.Web.Models.StatisticAnalysis.EfficiencyTrend;

    public class YieldStatisticsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IYieldAppService yieldAppService;

        public YieldStatisticsController(
            IYieldAppService yieldAppService,
            ICommonLookupAppService commonLookupAppService)
        {
            this.yieldAppService = yieldAppService;
            this.commonLookupAppService = commonLookupAppService;
        }

        public async Task<ActionResult> Index(int? tenantId)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new EfficiencyTrendsViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds = deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };

            return View("~/Views/StatisticAnalysis/YieldStatistics/Index.cshtml", model);
        }

        public async Task<PartialViewResult>  ShowQueryModal()
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
            return PartialView("~/Views/StatisticAnalysis/YieldStatistics/_ShowQueryModal.cshtml", modal);
        }

        [HttpPost]
        public async Task<JsonResult> GetMachineCapability(GetMachineYieldInputDto input)
        {
            var json = await this.yieldAppService.GetMachineCapability(input);
            return Json(json);
        }
    }
}