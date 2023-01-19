using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.DevicesMonitoring.MachineAlarms;

namespace Wimi.BtlCore.Web.Controllers.DevicesMonitoring
{

    public class MachineAlarmsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public MachineAlarmsController(
            ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
        }

        public PartialViewResult CreateOrEditAlarmMemoModal()
        {
            return this.PartialView("_CreateOrEditAlarmMemoModal");
        }

        //[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> Index(int? tenantId, int? targetDate)
        {
            this.ViewBag.TargetDate = targetDate;

            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var modal = new MachineAlarmsViewModal
            {
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                Machines = deviceGroupAndMachineWithPermissions.Machines
            };
            return this.View(modal);
        }
    }
}
