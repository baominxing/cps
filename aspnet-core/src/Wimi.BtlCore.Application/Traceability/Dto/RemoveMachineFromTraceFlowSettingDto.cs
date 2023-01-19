namespace Wimi.BtlCore.Traceability.Dto
{
    public class RemoveMachineFromTraceFlowSettingDto
    {
        public int TraceFlowSettingId { get; set; }

        public TraceRelatedMachineDto RelatedMachineDto { get; set; }
    }
}
