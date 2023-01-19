using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Cartons
{
    [Table("Cartons")]
    public class Carton : FullAuditedEntity
    {
        public Carton()
        {
            this.CartonRecords = new List<CartonRecord>();
        }

        [Required]
        [MaxLength(100)]
        [Comment("箱号")]
        public string CartonNo { get; set; }

        [Comment("设备组Id")]
        public int DeviceGroupId { get; set; }

        [ForeignKey("DeviceGroupId")]
        [Comment("设备组")]
        public virtual DeviceGroup DeviceGroup { get; set; }

        [Comment("最大包装数")]
        public int MaxPackingCount { get; set; }

        [Comment("实际包装数")]
        public int RealPackingCount { get; set; }

        [Comment("打印次数")]
        public int PrintLabelCount { get; set; }

        [ForeignKey("CartonId")]
        [Comment("包装列表")]
        public virtual ICollection<CartonRecord> CartonRecords { get; set; }
    }
}
