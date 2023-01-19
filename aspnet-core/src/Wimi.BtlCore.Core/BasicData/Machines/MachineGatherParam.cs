using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.Machines
{
    /// <summary>
    ///     设备采集参数维护
    /// </summary>
    [Table("MachineGatherParams")]
    public class MachineGatherParam : FullAuditedEntity<long>
    {
        /// <summary>
        ///     编码
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("参数编号")]
        public string Code { get; set; }

        /// <summary>
        /// 采集平台配置项的数据类型
        /// </summary>
        [Comment("参数数据类型")]
        public string DataType { get; set; }

        /// <summary>
        ///     参数显示方式
        /// </summary>
        [Comment("参数显示方式")]
        public EnumParamsDisplayStyle DisplayStyle { get; set; }

        /// <summary>
        ///     定义显示颜色
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("参数显示颜色")]
        public string Hexcode { get; set; }

        /// <summary>
        ///  是否显示(实时状态)
        /// </summary>
        [Comment("是否在实时状态页显示")]
        public bool IsShowForStatus { get; set; }

        /// <summary>
        ///  是否显示(运行参数)
        /// </summary>
        [Comment("是否在运行参数页显示")]
        public bool IsShowForParam { get; set; }

        /// <summary>
        ///     是否显示(看板)
        /// </summary>
        [Comment("是否在看板显示")]
        public bool IsShowForVisual { get; set; }

        /// <summary>
        ///     机器编码
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        [Comment("设备编号")]
        public string MachineCode { get; set; }

        [Comment("设备Id")]
        public long MachineId { get; set; }

        [Comment("参数最大值")]
        public double Max { get; set; }

        [Comment("参数最小值")]
        public double Min { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("参数名称")]
        public string Name { get; set; }

        /// <summary>
        ///     排列顺序
        /// </summary>
        [Comment("显示顺序")]
        public int SortSeq { get; set; }

        /// <summary>
        ///     参数单位
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("参数单位")]
        public string Unit { get; set; }

        [Comment("唯一标识(GUID)")]
        public Guid? MachineVariableId { get; set; }

        /// <summary>
        /// The default setting.
        /// </summary>
        public void DefaultSetting()
        {
            switch (this.Code)
            {
                case "STD::AlarmNo":
                case "STD::AlarmText":
                case "STD::Program":
                case "STD::YieldCounter":
                    this.DisplayStyle = EnumParamsDisplayStyle.BlockChart;
                    break;
            }
        }

        public void InsertDisplayStyleSetting()
        {
            switch (this.Code)
            {
                case "STD::AlarmNo":
                case "STD::AlarmText":
                case "STD::Program":
                case "STD::YieldCounter":
                    this.DisplayStyle = EnumParamsDisplayStyle.BlockChart;
                    break;
                default:
                    this.DisplayStyle = !string.IsNullOrEmpty(this.DataType) && this.DataType.ToLower().Equals("number")
                                            ? EnumParamsDisplayStyle.LineChart
                                            : EnumParamsDisplayStyle.BlockChart;
                    break;
            }
        }
    }
}
