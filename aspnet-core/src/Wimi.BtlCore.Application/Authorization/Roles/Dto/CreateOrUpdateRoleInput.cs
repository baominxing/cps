namespace Wimi.BtlCore.Authorization.Roles.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateOrUpdateRoleInputDto
    {
        [Required]
        public List<int> GrantedDeviceGroupPermissions { get; set; }

        [Required]
        public List<string> GrantedPermissionNames { get; set; }

        [Required]
        public RoleEditDto Role { get; set; }
    }
}