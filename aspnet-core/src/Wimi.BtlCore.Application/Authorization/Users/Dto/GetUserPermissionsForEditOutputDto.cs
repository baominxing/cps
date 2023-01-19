using System.Collections.Generic;
using Wimi.BtlCore.Authorization.Dto;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutputDto
    {
        public List<string> GrantedPermissionNames { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }
    }
}
