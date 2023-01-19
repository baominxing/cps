using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.Cutter
{
    [Table("CutterStates")]
    public class CutterStates : FullAuditedEntity
    {
        public CutterStates()
        {
            this.MachineGroupInfo = new MachineGroupInfo();
            if (this.Rate == 0)
            {
                this.Rate = 1;
            }
        }

        /// <summary>
        /// 计数方式
        /// </summary>
        [Required]
        [Comment("计数方式")]
        public EnumCountingMethod CountingMethod { get; set; }

        /// <summary>
        /// 刀具寿命状态
        /// </summary>
        [Required]
        [Comment("刀具寿命状态")]
        public EnumCutterLifeStates CutterLifeStatus { get; set; }

        /// <summary>
        /// 刀具型号Id
        /// </summary>
        [Required]
        [Comment("刀具型号Id")]
        public int CutterModelId { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Comment("刀具编号")]
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀位
        /// </summary>
        [Comment("刀位")]
        public int? CutterTValue { get; set; }

        /// <summary>
        /// 刀具类型Id
        /// </summary>
        [Required]
        [Comment("刀具类型Id")]
        public int CutterTypeId { get; set; }

        /// <summary>
        /// 刀具使用状态
        /// </summary>
        [Comment("刀具使用状态")]
        public EnumCutterUsedStates CutterUsedStatus { get; set; }

        /// <summary>
        /// 设备Id    
        /// </summary>
        [Comment("设备Id")]
        public int? MachineId { get; set; }

        [NotMapped]
        public MachineGroupInfo MachineGroupInfo { get; set; }

        /// <summary>
        /// 原始寿命
        /// </summary>
        [Required]
        [Comment("原始寿命")]
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

        /// <summary>
        /// 剩余寿命
        /// </summary>
        [Required]
        [Comment("剩余寿命")]
        public int RestLife { get; set; }

        /// <summary>
        /// 预警寿命
        /// </summary>
        [Required]
        [Comment("使用寿命")]
        public int UsedLife { get; set; }

        [Comment("预警寿命")]
        public int WarningLife { get; set; }

        /// <summary>
        /// 寿命消耗倍率
        /// </summary>
        [Required]
        [Comment("寿命消耗倍率")]
        public int Rate { get; set; }
    }
}