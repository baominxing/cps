using Abp.Authorization.Users;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    [AutoMapFrom(typeof(UserRole))]
    public class UserListRoleDto
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}