namespace Wimi.BtlCore.Cutter.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.AutoMapper;
    using Wimi.BtlCore.Cutter;

    [AutoMap(typeof(CutterStates))]
    public class CutterStatesEntityDto
    {
        /// <summary>
        /// 计数方式
        /// </summary>
        public EnumCountingMethod CountingMethod { get; set; }

        /// <summary>
        /// 刀具寿命状态
        /// </summary>
        public EnumCutterLifeStates CutterLifeStatus { get; set; }

        /// <summary>
        /// 刀具型号Id
        /// </summary>
        public int CutterModelId { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        [MaxLength(50)]
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀位
        /// </summary>
        public int? CutterTValue { get; set; }

        /// <summary>
        /// 刀具类型Id
        /// </summary>
        public int CutterTypeId { get; set; }

        /// <summary>
        /// 刀具使用状态
        /// </summary>
        public EnumCutterUsedStates? CutterUsedStatus { get; set; }

        public int? Id { get; set; }

        /// <summary>
        /// 设备Id    
        /// </summary>
        public int? MachineId { get; set; }

        /// <summary>
        /// 原始寿命
        /// </summary>
        public int OriginalLife { get; set; }

        [MaxLength(50)]
        public string Parameter1 { get; set; }

        [MaxLength(50)]
        public string Parameter10 { get; set; }

        [MaxLength(50)]
        public string Parameter2 { get; set; }

        [MaxLength(50)]
        public string Parameter3 { get; set; }

        [MaxLength(50)]
        public string Parameter4 { get; set; }

        [MaxLength(50)]
        public string Parameter5 { get; set; }

        [MaxLength(50)]
        public string Parameter6 { get; set; }

        [MaxLength(50)]
        public string Parameter7 { get; set; }

        [MaxLength(50)]
        public string Parameter8 { get; set; }

        [MaxLength(50)]
        public string Parameter9 { get; set; }

        /// <summary>
        /// 剩余寿命
        /// </summary>
        public int RestLife { get; set; }

        /// <summary>
        /// 预警寿命
        /// </summary>
        public int UsedLife { get; set; }

        public int WarningLife { get; set; }
    }
}