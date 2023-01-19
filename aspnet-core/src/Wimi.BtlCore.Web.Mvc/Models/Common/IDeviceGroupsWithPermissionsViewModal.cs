using System.Collections.Generic;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IDeviceGroupsWithPermissionsViewModal : IDeviceGroupsViewModal
    {
        List<int> GrantedGroupIds { get; set; }
    }
}
