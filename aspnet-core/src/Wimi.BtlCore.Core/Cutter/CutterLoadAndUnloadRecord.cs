namespace Wimi.BtlCore.Cutter
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// 装卸刀记录实体类
    /// </summary>
    [Table("CutterLoadAndUnloadRecords")]
    public class CutterLoadAndUnloadRecord : CreationAuditedEntity
    {
        /// <summary>
        /// 寿命计数方式（0：按次数，1：按时间）
        /// </summary>
        [Comment("寿命计数方式（0：按次数，1：按时间）")]
        public int CountingMethod { get; set; }

        /// <summary>
        /// 刀具型号
        /// </summary>
        [Comment("刀具型号")]
        public int CutterModelId { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        [Required]
        [StringLength(50)]
        [Comment("刀具编号")]
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀位（刀具T值）
        /// </summary>
        [Comment("刀位（刀具T值）")]
        public int? CutterTValue { get; set; }

        /// <summary>
        /// 刀具类型
        /// </summary>
        [Comment("刀具类型")]
        public int CutterTypeId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Comment("设备Id")]
        public int MachineId { get; set; }

        /// <summary>
        /// 0：卸刀，1：装刀
        /// </summary>
        [Comment("0：卸刀，1：装刀")]
        public int OperationType { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Comment("操作时间")]
        public DateTime? OperatorTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [Comment("操作人")]
        public long? OperatorUserId { get; set; }

        /// <summary>
        /// 原始寿命
        /// </summary>
        [Comment("原始寿命")]
        public int OriginalLife { get; set; }

        /// <summary>
        /// 刀具参数
        /// </summary>
        [StringLength(50)]
        [Comment("刀具参数1")]
        public string Parameter1 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数10")]
        public string Parameter10 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数2")]
        public string Parameter2 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数3")]
        public string Parameter3 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数4")]
        public string Parameter4 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数5")]
        public string Parameter5 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数6")]
        public string Parameter6 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数7")]
        public string Parameter7 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数8")]
        public string Parameter8 { get; set; }

        [StringLength(50)]
        [Comment("刀具参数9")]
        public string Parameter9 { get; set; }

        /// <summary>
        /// 剩余寿命
        /// </summary>
        [Comment("剩余寿命")]
        public int RestLife { get; set; }

        /// <summary>
        /// 已用寿命
        /// </summary>
        [Comment("已用寿命")]
        public int UsedLife { get; set; }
    }
}