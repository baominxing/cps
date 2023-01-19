namespace Wimi.BtlCore.BasicData.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Extensions;

    [AutoMapFrom(typeof(MachineGatherParam))]
    public class MachineGatherParamDto : EntityDto<long>
    {
        /// <summary>
        ///     编码
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        public string DataType { get; set; }

        /// <summary>
        ///     参数显示方式
        /// </summary>
        public EnumParamsDisplayStyle DisplayStyle { get; set; }

        public string DisplayStyleString => this.DisplayStyle.GetAttribute<DisplayAttribute>().Name;

        /// <summary>
        ///     定义显示颜色
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Hexcode { get; set; }

        /// <summary>
        ///     是否显示(实时状态)
        /// </summary>
        public bool IsShowForStatus { get; set; }

        /// <summary>
        ///  是否显示(运行参数)
        /// </summary>
        public bool IsShowForParam { get; set; }

        /// <summary>
        ///     是否显示(看板)
        /// </summary>
        public bool IsShowForVisual { get; set; }

        /// <summary>
        ///     机器编码
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MachineCode { get; set; }

        public long MachineId { get; set; }

        public double Max { get; set; }

        public double Min { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Name { get; set; }

        /// <summary>
        ///     排列顺序
        /// </summary>
        public int SortSeq { get; set; }

        public int? TenantId { get; set; }

        /// <summary>
        ///     显示单位
        /// </summary>
        public string Unit { get; set; }
    }
}