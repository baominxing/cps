using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Trace
{
    [Table("TraceCatalogs")]
    public class TraceCatalog : Entity<long>
    {
        [Comment("工件编号")]
        [MaxLength(BtlCoreConsts.MaxLength*2)]
        //[Index("IX_TraceCatalog", 1)]
        public string PartNo { get; set; }

        [Comment("上线时间")]
        //[Index("IX_TraceCatalog", 2)]
        public DateTime OnlineTime { get; set; }

        [Comment("下线时间")]
        //[Index("IX_TraceCatalog", 3)]
        public DateTime? OfflineTime { get; set; }

        [Comment("设备组ID")]
        public int DeviceGroupId { get; set; }

        [Comment("是否合格")]
        public bool? Qualified { get; set; }

        [Comment("是否返工")]
        public bool? IsReworkPart { get; set; }

        [Comment("设备班次详情Id")]
        public int MachineShiftDetailId { get; set; }

        [Comment("计划Id")]
        public int PlanId { get; set; }

        [Comment("归档表")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ArchivedTable { get; set; }

    }
}
