using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;

namespace Wimi.BtlCore.Web.Models.App
{
    public class TenantDashboardViewModal
    {
        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<FlatMachineDto> MachineList { get; set; }

        public List<StateInfo> StatusInfoList { get; set; }

        public bool IsShiftExpiry { get; set; }

        public string MachineShiftNotSchedulings { get; set; }
    }
}
