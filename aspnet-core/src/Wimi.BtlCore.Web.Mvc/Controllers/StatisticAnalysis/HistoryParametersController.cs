using System.Threading.Tasks;

using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.RealtimeIndicators.Parameters;
using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.HistoryParameter;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    [AbpAuthorize(PermissionNames.Pages_HistoryParameters)]
    public class HistoryParametersController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IParamtersAppService paramtersAppService;

        public HistoryParametersController(
            ICommonLookupAppService commonLookupAppService, 
            IParamtersAppService paramtersAppService)
        {
            this.paramtersAppService = paramtersAppService;
            this.commonLookupAppService = commonLookupAppService;
        }

        [HttpPost]
        public async Task<JsonResult> GetHistoryParameterList(HistoryParamtersInputDto input)
        {
            var json = await this.paramtersAppService.GetHistoryParamtersList(input);
            return this.Json(json);
        }

        // GET: HistoryParameters
        public async Task<ActionResult> Index()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new HistoryParametersViewModel
                            {
                                DeviceGroups =
                                    deviceGroupAndMachineWithPermissions.DeviceGroups, 
                                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds, 
                                Machines = deviceGroupAndMachineWithPermissions.Machines
                            };
            return this.View("~/Views/StatisticAnalysis/HistoryParameters/Index.cshtml", model);
        }

        public PartialViewResult ViewComparison()
        {
            return this.PartialView("~/Views/StatisticAnalysis/HistoryParameters/_ViewComparisonModal.cshtml");
        }
    }
}