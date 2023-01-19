using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Traceability.Dto
{
    [AutoMap(typeof(TraceRelatedMachine))]
    public class TraceRelatedMachineDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int TraceFlowSettingId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        public string WorkingStationCode { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string WorkingStationDisplayName { get; set; }
    }
}
