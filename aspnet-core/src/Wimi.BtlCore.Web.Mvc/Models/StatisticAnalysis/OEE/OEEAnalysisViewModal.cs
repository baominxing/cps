﻿namespace Wimi.BtlCore.Web.Models.OEE.StatisticAnalysis
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Web.Models.Common;

    public class OEEAnalysisViewModal: IDeviceGroupAndMachineWithPermissionsViewModal
    {
        public List<FlatDeviceGroupDto> DeviceGroups { get; set; }

        public List<int> GrantedGroupIds { get; set; }

        public List<FlatMachineDto> Machines { get; set; }
    }
}