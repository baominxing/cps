using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Cartons
{
    [Table("CartonSettings")]
    public class CartonSetting : FullAuditedEntity
    {
        [Comment("设备组Id")]
        public int DeviceGroupId { get; set; }

        [ForeignKey("DeviceGroupId")]
        [Comment("设备组")]
        public virtual DeviceGroup DeviceGroup { get; set; }

        [Comment("最大装箱数")]
        public int MaxPackingCount { get; set; }

        [Comment("是否打印")]
        public bool IsPrint { get; set; }

        [Comment("打印机名称")]
        public string PrinterName { get; set; }

        [Comment("自动生成箱码")]
        public bool AutoCartonNo { get; set; }

        [Comment("箱号规则Id")]
        public int CartonRuleId { get; set; }

        [Comment("只能装合格件")]
        public bool IsGoodOnly { get; set; }

        [Comment("是否自动打印")]
        public bool IsAutoPrint { get; set; }

        [Comment("禁止跳序")]
        public bool ForbidHopSequence { get; set; }

        [Comment("禁止重复装箱")]
        public bool ForbidRepeatPacking { get; set; }

        [Comment("是否允许拆箱重装")]
        public bool IsUnpackingRedo { get; set; }

        [Comment("是否允许打印后拆箱重装")]
        public bool IsUnpackingAfterPrint { get; set; }

        [Comment("是否需要终检")]
        public bool IsFinalTest { get; set; }

        [Comment("必经流程")]
        public bool HasToFlow { get; set; }

        [Comment("必经流程Id")]
        public string FlowIds { get; set; }
    }
}
