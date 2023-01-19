using System;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class GetUserForEditOutputDto
    {
        public Guid? ProfilePictureId { get; set; }

        public UserRoleDto[] Roles { get; set; }

        public UserEditDto User { get; set; }
    }
}
