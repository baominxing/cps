using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Parameters.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.DevicesMonitoring.MachineStates
{
    public class ParamtersChartViewModel : IDeviceGroupAndMachineWithPermissionsViewModal
    {
        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }

        public string MachineCode { get; set; }

        public List<FlatMachineDto> Machines { get; set; }

        public GetParamtersListDto ParamtersListDto { get; set; }
    }
}
