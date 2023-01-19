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
    [AbpMvcAuthorize(PermissionNames.Pages_DevicesRealtimeAlarms)]
    public class MachineRealtimeAlarmsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IRepository<StateInfo> statusInfoRepository;

        public MachineRealtimeAlarmsController(
            ICommonLookupAppService commonLookupAppService,
            IRepository<StateInfo> statusInfoRepository)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.statusInfoRepository = statusInfoRepository;
        }

        // GET: MachineRealtimeAlarms
        public async Task<ActionResult> Index()
        {
            var deviceGroupWithPermissions = await this.commonLookupAppService.GetDeviceGroupWithPermissions();

            var model = new MachineStatesViewModel
            {
                StateInfoList = await this.statusInfoRepository.GetAllListAsync(),
                DeviceGroups = deviceGroupWithPermissions.DeviceGroups,
                GrantedGroupIds = deviceGroupWithPermissions.GrantedGroupIds
            };

            return this.View("~/Views/DevicesMonitoring/MachineRealtimeAlarms/Index.cshtml", model);
        }
    }
}
