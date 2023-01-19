using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.App;

namespace Wimi.BtlCore.Web.Controllers.Dashboard
{
    public class DashboardController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<StateInfo> statusInfoRepository;
        private readonly MachineShiftEffectiveIntervalManager machineShiftEffectiveIntervalManager;

        public DashboardController(
            ICommonLookupAppService commonLookupAppService,
            IRepository<StateInfo> statusInfoRepository,
            IRepository<DeviceGroup> deviceGroupRepository, MachineShiftEffectiveIntervalManager machineShiftEffectiveIntervalManager)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.statusInfoRepository = statusInfoRepository;
            this.DeviceGroupRepository = deviceGroupRepository;
            this.machineShiftEffectiveIntervalManager = machineShiftEffectiveIntervalManager;
        }

        /// <summary>
        /// Gets the device group repository.
        /// </summary>
        public IRepository<DeviceGroup> DeviceGroupRepository { get; }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        //[AbpMvcAuthorize(PermissionNames.Pages_Tenant_Dashboard)]
        public async Task<ActionResult> Index()
        {
            var deviceGroupWithPermissions = await this.commonLookupAppService.GetDeviceGroupWithPermissions();
            var group =
                deviceGroupWithPermissions.DeviceGroups.Where(
                    g => deviceGroupWithPermissions.GrantedGroupIds.Contains(g.Id) && g.MemberCount > 0).ToList();

            var modal = new TenantDashboardViewModal
            {
                StatusInfoList = await this.statusInfoRepository.GetAllListAsync(),
                DeviceGroups = this.FormartGroupName(group),
                IsShiftExpiry = await this.machineShiftEffectiveIntervalManager.IsMachineShiftExpiry(),
                MachineShiftNotSchedulings = await this.machineShiftEffectiveIntervalManager.ListMachineShiftNotScheduling()
            };

            return this.View("Index", modal);
        }


        public PartialViewResult MachineShiftWarning()
        {
            return this.PartialView("_MachineShiftWarning");
        }

        [AbpMvcAuthorize]
        public PartialViewResult ShortcutMenuModal()
        {
            return this.PartialView("_ShortcutMenuModal");
        }

        //[AbpMvcAuthorize(PermissionNames.Pages_Tenant_Dashboard)]
        public async Task<PartialViewResult> ShowMachineEfficiencyModal(int groupId)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new TenantDashboardViewModal
            {
                MachineList = deviceGroupAndMachineWithPermissions.Machines.Where(m => m.DeviceGroupId == groupId).ToList(),
                StatusInfoList = await this.statusInfoRepository.GetAllListAsync()
            };
            return this.PartialView("_ShowMachineEfficiencyModal", null);
        }

        /// <summary>
        /// The formart group name.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        private List<FlatDeviceGroupDto> FormartGroupName(IEnumerable<FlatDeviceGroupDto> group)
        {
            var result = new List<FlatDeviceGroupDto>();
            foreach (var item in group)
            {
                var name = item.DisplayName;
                if (item.Code.Length > 5)
                {
                    // code like 00001.00001
                    var parentNode = this.DeviceGroupRepository.GetAll().First(s => s.Code.Equals(item.Code.Substring(0, 5)));
                    name = $"[{parentNode.DisplayName}] - {name}";
                }

                item.DisplayNameWithGroup = name;
                result.Add(item);
            }

            return result;
        }
    }
}
