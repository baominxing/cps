using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.DevicesMonitoring.MachineStates
{
    public class MachineStatesViewModel : IDeviceGroupAndMachineWithPermissionsViewModal
    {
        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }

        public List<StateInfo> StateInfoList { get; set; }

        public List<FlatMachineDto> Machines { get; set; }
    }
}
