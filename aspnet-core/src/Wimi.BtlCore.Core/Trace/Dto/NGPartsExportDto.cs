using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Trace.Dto
{
    public class NGPartsExportDto
    {
        public long Id { get; set; }

        public int DeviceGroupId { get; set; }

        public string DeviceGroupName { get; set; }

        public string PartNo { get; set; }

        public DateTime OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string StationCode { get; set; }

        public string StationName { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string ShiftName { get; set; }

        public string State { get; set; }

        /// <summary>
        /// NG原因列表，以逗号分隔
        /// </summary>
        public string DefectiveReasonNames { get; set; }
    }

    public class DefectReasonExportDto
    {
        public string PartNo { get; set; }

        public int MachineId { get; set; }

        public IEnumerable<string> DefectiveReasonNames { get; set; } = new List<string>();
    }

    public class DefectReasonName
    {
        public string PartNo { get; set; }

        public int DefectiveMachineId { get; set; }

        public string Name { get; set; }
    }
}
