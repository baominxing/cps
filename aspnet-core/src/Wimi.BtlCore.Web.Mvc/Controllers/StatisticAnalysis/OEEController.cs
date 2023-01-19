using System.Threading.Tasks;

using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.OEE.StatisticAnalysis;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    [AbpAuthorize(PermissionNames.Pages_OEE)]
    public class OEEController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public OEEController(ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
        }

        // GET: OEE
        public async Task<ActionResult> Index()
        {
            var deviceGroupAndMachineWithPermissions = await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new OEEAnalysisViewModal()
                            {
                                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                                GrantedGroupIds = deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                                Machines = deviceGroupAndMachineWithPermissions.Machines
                            };
            return this.View("~/Views/StatisticAnalysis/OEE/Index.cshtml", model);
        }
    }
}