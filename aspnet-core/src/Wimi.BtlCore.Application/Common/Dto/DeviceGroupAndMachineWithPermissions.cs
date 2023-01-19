namespace Wimi.BtlCore.Common.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;

    public class DeviceGroupAndMachineWithPermissionsDto : DeviceGroupWithPermissionsDto
    {
        public DeviceGroupAndMachineWithPermissionsDto()
        {
            this.Machines = new List<FlatMachineDto>();
        }

        public DeviceGroupAndMachineWithPermissionsDto(DeviceGroupWithPermissionsDto deviceGroupWithPermissions)
            : this()
        {
            this.GrantedGroupIds = deviceGroupWithPermissions.GrantedGroupIds;
            this.DeviceGroups = deviceGroupWithPermissions.DeviceGroups;
        }

        public DeviceGroupAndMachineWithPermissionsDto(
            DeviceGroupWithPermissionsDto deviceGroupWithPermissions, 
            List<FlatMachineDto> machines)
            : this(deviceGroupWithPermissions)
        {
            this.Machines = machines;
        }

        public DeviceGroupAndMachineWithPermissionsDto(
            List<int> grantedGroupIds, 
            List<FlatDeviceGroupDto> deviceGroups, 
            List<FlatMachineDto> machines)
            : base(grantedGroupIds, deviceGroups)
        {
            this.Machines = machines;
        }

        public List<FlatMachineDto> Machines { get; set; }
    }
}