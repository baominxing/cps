using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.EfficiencyTrend;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    public class EfficiencyTrendsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IEfficiencyTrendsAppService efficiencyTrendsAppService;

        public EfficiencyTrendsController(
            ICommonLookupAppService commonLookupAppService, 
            IEfficiencyTrendsAppService efficiencyTrendsAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.efficiencyTrendsAppService = efficiencyTrendsAppService;
        }

        [HttpPost]
        public async Task<JsonResult> GetEfficiencyTrendasList(EfficiencyTrendsInputDto input)
        {
            var json = await this.efficiencyTrendsAppService.GetEfficiencyTrendasList(input);
            return this.Json(json);
        }

        // GET: EfficiencyTrends
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

            return this.View("~/Views/StatisticAnalysis/EfficiencyTrends/Index.cshtml", model);
        }

        public async Task<PartialViewResult> FilterModal()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new EfficiencyTrendsViewModel
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.PartialView("~/Views/StatisticAnalysis/EfficiencyTrends/_FilterModal.cshtml", modal);
        }
    }
}