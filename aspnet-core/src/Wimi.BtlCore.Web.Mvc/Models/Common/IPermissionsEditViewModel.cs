using System.Collections.Generic;
using Wimi.BtlCore.Authorization.Dto;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}