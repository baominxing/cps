using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Cutter.CutterLoadAndUnloadRecord;

namespace Wimi.BtlCore.Web.Controllers.Cutter
{
    public class CutterLoadAndUnloadRecordController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        public CutterLoadAndUnloadRecordController(ICommonLookupAppService commonLookupAppService)
        {
            this.commonLookupAppService = commonLookupAppService;
        }

        // GET: CutterLoadAndUnloadRecord
        public async Task<ActionResult> Index()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new CutterLoadAndUnloadRecordViewMOdal
            {
                DeviceGroups =
                                    deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions
                                    .GrantedGroupIds,
                Machines =
                                    deviceGroupAndMachineWithPermissions.Machines
            };
            return this.View("~/Views/Cutter/CutterLoadAndUnloadRecord/Index.cshtml", model);
 
        }
    }
}
