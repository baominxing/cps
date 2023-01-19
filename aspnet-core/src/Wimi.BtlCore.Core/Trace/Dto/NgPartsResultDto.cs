using System;

namespace Wimi.BtlCore.Trace.Dto
{
    public class NgPartsResultDto
    {
        public long Id { get; set; }

        public int DeviceGroupId { get; set; }

        public string DeviceGroupName { get; set; }

        public string PartNo { get; set; }

        public DateTime? OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string StationCode { get; set; }

        public string StationName { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string ShiftName { get; set; }

        public string State { get; set; }
    }
}
