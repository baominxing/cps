namespace Wimi.BtlCore.Web.Models.StatisticAnalysis.YieldAnalysisStatistics
{
    using System.Collections.Generic;

    using Castle.Components.DictionaryAdapter;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Web.Models.Common;

    public class GetQueryConditionsViewModel : IDeviceGroupAndMachineWithPermissionsViewModal
    {
        public GetQueryConditionsViewModel()
        {
            this.DeviceGroups = new EditableList<FlatDeviceGroupDto>();
            this.GrantedGroupIds = new EditableList<int>();
            this.Machines = new EditableList<FlatMachineDto>();
        }

        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }

        public List<FlatMachineDto> Machines { get; set; }
    }
}