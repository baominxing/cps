using System.Collections.Generic;
using Wimi.BtlCore.DeviceGroups.Dto;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IDeviceGroupPermissionsEditViewModel
    {
        List<DeviceGroupDto> DeviceGroups { get; set; }

        List<int> GrantedDeviceGroupPermissions { get; set; }
    }
}
