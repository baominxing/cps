using Abp.AutoMapper;
using Abp.ObjectMapping;
using System.Collections.Generic;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Authorization.Users.Dto;
using Wimi.BtlCore.Common.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutputDto))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutputDto, IPermissionsEditViewModel
    {
        public UserPermissionsEditViewModel(GetUserPermissionsForEditOutputDto output, User user)
        {
            this.User = user;
            this.GrantedPermissionNames = output.GrantedPermissionNames;
            this.Permissions = output.Permissions;
        }

        public List<DeviceGroupPermissionDto> DeviceGroupPermissions { get; set; }

        public User User { get; private set; }
    }
}
