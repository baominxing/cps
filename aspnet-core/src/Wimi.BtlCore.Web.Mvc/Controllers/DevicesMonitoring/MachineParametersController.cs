using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.DevicesMonitoring.MachineStates;

namespace Wimi.BtlCore.Web.Controllers.DevicesMonitoring
{
    [AbpMvcAuthorize(PermissionNames.Pages_DevicesMonitoring)]
    public class MachineParametersController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public MachineParametersController(
            ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
        }

        // GET: MachineStates
        public async Task<ActionResult> Index(string machineTreeId)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new ParamtersChartViewModel
            {
                MachineCode = machineTreeId,
                Machines = deviceGroupAndMachineWithPermissions.Machines,
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds
            };
            return this.View("~/Views/DevicesMonitoring/MachineParameters/Index.cshtml", model);
        }
    }
}
