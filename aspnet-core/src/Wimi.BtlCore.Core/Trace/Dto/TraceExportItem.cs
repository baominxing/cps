namespace Wimi.BtlCore.Trace.Dto
{
    public class TraceExportItem
    {
        public string PartNo { get; set; }

        public string TraceStates { get; set; }

        public bool? Qualified { get; set; }

        public string ShiftItemName { get; set; }

        public string MachineName { get; set; }

        public string FlowName { get; set; }

        public string EntryTime { get; set; }

        public string LeftTime { get; set; }

        public string State { get; set; }
    }
}
