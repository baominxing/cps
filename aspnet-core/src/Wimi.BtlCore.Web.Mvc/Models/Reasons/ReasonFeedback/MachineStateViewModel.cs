namespace Wimi.BtlCore.Web.Models.Reasons.ReasonFeedback
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.StateInfos;
    using Wimi.BtlCore.Web.Models.Common;

    public class MachineStateViewModel : IDeviceGroupsWithPermissionsViewModal
    {
        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }

        public List<StateInfo> StateInfoList { get; set; }
    }
}