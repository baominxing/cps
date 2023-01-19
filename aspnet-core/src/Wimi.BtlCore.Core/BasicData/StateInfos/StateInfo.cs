using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.BasicData.StateInfos
{
    [Table("StateInfos")]
    public class StateInfo : Entity
    {
        /// <summary>
        /// Stop/Run/Free/Offline/Debug..
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("状态编号")]
        public string Code { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("状态名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("状态颜色")]
        public string Hexcode { get; set; }

        /// <summary>
        /// 是否计划内
        /// </summary>
        [Comment("是否计划内")]
        public bool IsPlaned { get; set; }

        /// <summary>
        /// 静态状态无法编辑和删除，由程序创建
        /// </summary>
        [Comment("静态状态无法编辑和删除，由程序创建")]
        public bool IsStatic { get; set; }

        /// <summary>
        /// 原始采集编号
        /// </summary>
        [Comment("原始采集编号")]
        public int? OriginalCode { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        [Comment("状态类型")]
        public EnumMachineStateType Type { get; set; }
    }
}
