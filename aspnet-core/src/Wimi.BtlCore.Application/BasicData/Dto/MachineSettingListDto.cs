namespace Wimi.BtlCore.BasicData.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMap(typeof(Machine))]
    public class MachineSettingListDto : AuditedEntity<int?>
    {
        /// <summary>
        ///     机器编号
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        /// <summary>
        ///     机器描述
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string Desc { get; set; }

        /// <summary>
        ///     机器最终使用的客户Id
        /// </summary>
        public int? FinalTenantId { get; set; }

        /// <summary>
        ///     图片报错Id，在表AppBinaryObjects中
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsActive { get; set; }

        public int MachineTypeId { get; set; }

        public string MachineTypeName { get; set; }

        /// <summary>
        ///     机器名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Name { get; set; }

        /// <summary>
        ///     机器排列顺序
        /// </summary>
        public int SortSeq { get; set; }

        public EnumMachineState State { get; set; }

        /// <summary>
        ///     客户Id
        /// </summary>
        public int? TenantId { get; set; }

        [MaxLength(BtlCoreConsts.IPAdressLength)]
        public string IpAddress { get; set; }

        public int? TcpPort { get; set; }
    }
}