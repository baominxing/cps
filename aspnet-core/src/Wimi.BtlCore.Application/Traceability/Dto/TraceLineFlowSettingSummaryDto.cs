using Abp.AutoMapper;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Traceability.Dto
{
    [AutoMapFrom(typeof(TraceFlowSetting))]
    public class TraceLineFlowSettingSummaryDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public FlowType FlowType { get; set; }
    }
}
