using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Trace.Repository.Dtos
{
    [AutoMapFrom(typeof(TraceCatalog))]
    public class TraceCatalogDto
    {
        public long Id { get; set; }

        public int PlanId { get; set; }

        public int MachineId { get; set; }

        public string WorkingStationCode { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string PartNo { get; set; }

        public DateTime OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public int DeviceGroupId { get; set; }

        public bool? Qualified { get; set; }

        public bool? IsReworkPart { get; set; }

        public int MachineShiftDetailId { get; set; }

        public bool IsOffline => OfflineTime.HasValue;

        public string Station { get; internal set; }

        public DateTime? EntryTime { get; internal set; }

        public DateTime? LeftTime { get; internal set; }

        public string ArchivedTable { get; set; }

    }
}
