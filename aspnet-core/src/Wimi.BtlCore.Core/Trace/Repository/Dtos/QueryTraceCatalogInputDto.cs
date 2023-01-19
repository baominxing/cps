using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Trace.Repository.Dtos
{
    public class QueryTraceCatalogInputDto
    {
        public List<int> MachineId { get; set; }

        public int DefectiveMachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string StationCode { get; set; }

        public int? Id { get; set; }

        public string PartNo { get; set; }

        public int? DeviceGroupId { get; set; }

        public DateTime? StartFirstTime { get; set; }

        public DateTime? StartLastTime { get; set; }

        public DateTime? EndFirstTime { get; set; }

        public DateTime? EndLastTime { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public long NgPartCatlogId { get; set; }

        public string ArchivedTable { get; set; }
    }
}
