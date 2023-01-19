namespace Wimi.BtlCore.Authorization.Roles.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.Authorization.Dto;
    using Wimi.BtlCore.DeviceGroups.Dto;

    public class GetRoleForEditOutputDto
    {
        public List<DeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedDeviceGroupPermissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public RoleEditDto Role { get; set; }
    }
}