using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    [Table("FmsCutters")]
    public class FmsCutter : FullAuditedEntity
    {
        public FmsCutter()
        {
            this.Items = new List<FmsCutterExtend>();
        }

        /// <summary>
        /// 机床编号
        /// </summary>
        [DefaultColumnOrder(0, "MachineName")]
        [Comment("设备Id")]
        public int? MachineId { get; set; }

        /// <summary>
        /// 刀库编号
        /// </summary>
        [DefaultColumnOrder(1)]
        [Comment("刀库编号")]
        public string CutterStockNo { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        [DefaultColumnOrder(2)]
        [Comment("刀具编号")]
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀套编号
        /// </summary>
        [DefaultColumnOrder(3)]
        [Comment("刀套编号")]
        public string CutterCase { get; set; }

        /// <summary>
        /// 刀具类型
        /// </summary>
        [DefaultColumnOrder(4)]
        [Comment("刀具类型")]
        public string Type { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        [DefaultColumnOrder(5)]
        [Comment("长度")]
        public decimal Length { get; set; }

        /// <summary>
        /// 直径
        /// </summary>
        [DefaultColumnOrder(6)]
        [Comment("直径")]
        public decimal Diameter { get; set; }

        /// <summary>
        /// 补偿号
        /// </summary>
        [DefaultColumnOrder(7)]
        [Comment("补偿号")]
        public string CompensateNo { get; set; }

        /// <summary>
        /// 长度补偿
        /// </summary>
        [DefaultColumnOrder(8)]
        [Comment("长度补偿")]
        public decimal LengthCompensate { get; set; }

        /// <summary>
        /// 直径补偿
        /// </summary>
        [DefaultColumnOrder(9)]
        [Comment("直径补偿")]
        public decimal DiameterCompensate { get; set; }

        /// <summary>
        /// 预设寿命
        /// </summary>
        [DefaultColumnOrder(10)]
        [Comment("预设寿命")]
        public decimal OriginalLife { get; set; }

        /// <summary>
        /// 当前寿命
        /// </summary>
        [DefaultColumnOrder(11)]
        [Comment("当前寿命")]
        public decimal CurrentLife { get; set; }

        /// <summary>
        /// 预警寿命
        /// </summary>
        [DefaultColumnOrder(12)]
        [Comment("预警寿命")]
        public decimal WarningLife { get; set; }

        /// <summary>
        /// 使用类型
        /// </summary>
        [DefaultColumnOrder(13)]
        [Comment("使用类型")]
        public EnumFmsUseType UseType { get; set; }

        /// <summary>
        /// 计数类型
        /// </summary>
        [DefaultColumnOrder(14)]
        [Comment("计数类型")]
        public EnumFmsCutterCountType CountType { get; set; }

        /// <summary>
        /// 刀具状态
        /// </summary>
        [DefaultColumnOrder(15)]
        [Comment("刀具状态")]
        public EnumFmsCutterState State { get; set; }

        [Comment("刀具列表")]
        public virtual ICollection<FmsCutterExtend> Items { get; set; }

    }
}
