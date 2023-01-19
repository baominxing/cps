using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.DevicesMonitoring.MachineStates;

namespace Wimi.BtlCore.Web.Controllers.DevicesMonitoring
{
    [AbpMvcAuthorize(PermissionNames.Pages_DevicesMonitoring)]
    public class MachineStatesController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IRepository<StateInfo> statusInfoRepository;

        public MachineStatesController(
            IRepository<StateInfo> statusInfoRepository,
            ICommonLookupAppService commonLookupAppService)
        {
            this.statusInfoRepository = statusInfoRepository;
            this.commonLookupAppService = commonLookupAppService;
        }

        // GET: MachineStates
        public async Task<ActionResult> Index(int? tenantId)
        {
            var deviceGroupWithPermissions = await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var model = new MachineStatesViewModel
            {
                StateInfoList = await this.statusInfoRepository.GetAllListAsync(),
                DeviceGroups = deviceGroupWithPermissions.DeviceGroups,
                GrantedGroupIds = deviceGroupWithPermissions.GrantedGroupIds,
                Machines = deviceGroupWithPermissions.Machines
            };

            return this.View("~/Views/DevicesMonitoring/MachineStates/Index.cshtml", model);
        }
    }
}
