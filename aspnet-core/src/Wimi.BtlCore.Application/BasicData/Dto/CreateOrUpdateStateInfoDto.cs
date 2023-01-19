namespace Wimi.BtlCore.BasicData.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.StateInfos;

    [AutoMap(typeof(StateInfo))]
    public class CreateOrUpdateStateInfoDto : AuditedEntity<int?>
    {
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Required]
        public string Hexcode { get; set; }

        /// <summary>
        /// 是否计划内
        /// </summary>
        public bool IsPlaned { get; set; }

        /// <summary>
        /// 静态状态无法编辑和删除，由程序创建
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public EnumMachineStateType Type { get; set; }
    }
}