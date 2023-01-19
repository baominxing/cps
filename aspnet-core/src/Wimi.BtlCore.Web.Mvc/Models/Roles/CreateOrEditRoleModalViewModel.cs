
using Abp.AutoMapper;
using AutoMapper;
using Wimi.BtlCore.Authorization.Roles.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.Roles
{
    [AutoMapFrom(typeof(GetRoleForEditOutputDto))]
    public class CreateOrEditRoleModalViewModel : GetRoleForEditOutputDto,
                                                  IPermissionsEditViewModel,
                                                  IDeviceGroupPermissionsEditViewModel
    {
 
        public CreateOrEditRoleModalViewModel(GetRoleForEditOutputDto output)
        {
            this.DeviceGroups = output.DeviceGroups;
            this.GrantedDeviceGroupPermissions = output.GrantedDeviceGroupPermissions;
            this.GrantedPermissionNames = output.GrantedPermissionNames;
            this.Permissions = output.Permissions;
            this.Role = output.Role;
        }

        public bool IsEditMode
        {
            get
            {
                return this.Role.Id.HasValue;
            }
        }
    }
}
