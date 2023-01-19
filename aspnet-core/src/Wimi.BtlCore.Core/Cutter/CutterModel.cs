namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("CutterModels")]
    public class CutterModel : FullAuditedEntity
    {
        public EnumCountingMethod CountingMethod { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("刀具编号前缀")]
        public string CutterNoPrefix { get; set; }

        [Comment("刀具类型Id")]
        public int CutterTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }

        [Comment("初始寿命")]
        public int OriginalLife { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数1")]
        public string Parameter1 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数10")]
        public string Parameter10 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数2")]
        public string Parameter2 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数3")]
        public string Parameter3 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数4")]
        public string Parameter4 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数5")]
        public string Parameter5 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数6")]
        public string Parameter6 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数7")]
        public string Parameter7 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数8")]
        public string Parameter8 { get; set; }

        [MaxLength(50)]
        [Comment("刀具参数9")]
        public string Parameter9 { get; set; }

        [Comment("预警寿命")]
        public int WarningLife { get; set; }

        public string GetCutterNo(int maxSerialNo = 0)
        {
            return $"{this.CutterNoPrefix}{(maxSerialNo + 1).ToString().PadLeft(6, '0')}";
        }
    }
}