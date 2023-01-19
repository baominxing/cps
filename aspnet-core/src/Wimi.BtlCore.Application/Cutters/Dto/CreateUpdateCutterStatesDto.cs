using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Cutter;

namespace Wimi.BtlCore.Cutter.Dto
{
    [AutoMap(typeof(CutterStates))]
    public class CreateUpdateCutterStatesDto
    {
        public int? Id { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        [MaxLength(50)]
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀具使用状态
        /// </summary>
        public EnumCutterUsedStates? CutterUsedStatus { get; set; }

        /// <summary>
        /// 刀具类型Id
        /// </summary>
        public int CutterTypeId { get; set; }

        /// <summary>
        /// 刀具型号Id
        /// </summary>
        public int CutterModelId { get; set; }
        
        /// <summary>
        /// 计数方式
        /// </summary>
        public EnumCountingMethod CountingMethod { get; set; }

        /// <summary>
        /// 原始寿命
        /// </summary>
        public int OriginalLife { get; set; }

        /// <summary>
        /// 剩余寿命
        /// </summary>
        public int RestLife { get; set; }

        /// <summary>
        /// 预警寿命
        /// </summary>
        public int UsedLife { get; set; }

        public int WarningLife { get; set; }

        public string Parameter1 { get; set; }

        public string Parameter10 { get; set; }

        public string Parameter2 { get; set; }

        public string Parameter3 { get; set; }

        public string Parameter4 { get; set; }

        public string Parameter5 { get; set; }

        public string Parameter6 { get; set; }

        public string Parameter7 { get; set; }

        public string Parameter8 { get; set; }

        public string Parameter9 { get; set; }
    }
}
