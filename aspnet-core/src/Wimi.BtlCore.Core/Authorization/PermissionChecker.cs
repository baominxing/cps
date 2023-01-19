using Abp.Authorization;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Authorization.Users;

namespace Wimi.BtlCore.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
