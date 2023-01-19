using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Trace
{
    [Table("TraceRelatedMachines")]
    public class TraceRelatedMachine : Entity
    {
        [Comment("设备ID")]
        public int MachineId { get; set; }

        [Comment("设备编号")]
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        public string MachineCode { get; set; }

        [Comment("流程设定ID")]
        public int TraceFlowSettingId { get; set; }

        public virtual TraceFlowSetting TraceFlowSetting { get; set; }

        [Comment("工位编号")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string WorkingStationCode { get; set; }

        [Comment("工位名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string WorkingStationDisplayName { get; set; }
    }
}
