using System.Collections.Generic;
using Wimi.BtlCore.Trace.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class TraceExportItemDto
    {
        public string PartNo { get; set; }

        public string TraceStates { get; set; }

        public string ShiftItemName { get; set; }

        public int Length { get; set; }

        public string FlowName { get; set; }

        public bool? Qualified { get; set; }

        public List<TraceExportItem> DetailItems { get; set; }
    }
}
