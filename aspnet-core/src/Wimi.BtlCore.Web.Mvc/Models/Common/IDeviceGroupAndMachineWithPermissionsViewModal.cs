using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IDeviceGroupAndMachineWithPermissionsViewModal : IDeviceGroupsWithPermissionsViewModal
    {
        List<FlatMachineDto> Machines { get; set; }
    }
}
