using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Common.Dto
{
    public class DeviceGroupWithPermissionsDto
    {
        public DeviceGroupWithPermissionsDto()
        {
            this.GrantedGroupIds = new List<int>();
            this.DeviceGroups = new List<FlatDeviceGroupDto>();
        }

        public DeviceGroupWithPermissionsDto(List<int> grantedGroupIds, List<FlatDeviceGroupDto> deviceGroups)
        {
            this.GrantedGroupIds = grantedGroupIds;
            this.DeviceGroups = deviceGroups;
        }

        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }
    }
}