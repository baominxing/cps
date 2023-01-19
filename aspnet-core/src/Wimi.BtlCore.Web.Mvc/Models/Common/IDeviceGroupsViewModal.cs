using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IDeviceGroupsViewModal
    {
        List<FlatDeviceGroupDto> DeviceGroups { get; set; }
    }
}
