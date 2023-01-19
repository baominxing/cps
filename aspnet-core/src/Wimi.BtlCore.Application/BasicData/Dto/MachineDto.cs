namespace Wimi.BtlCore.BasicData.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMap(typeof(Machine))]
    public class MachineDto : FullAuditedEntity
    {
        /// <summary>
        ///     机器编号
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        public string Code { get; set; }

        /// <summary>
        ///     机器描述
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string Desc { get; set; }

        /// <summary>
        ///     图片报错Id，在表AppBinaryObjects中
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     设备类型Id
        /// </summary>
        public int MachineTypeId { get; set; }

        /// <summary>
        ///     机器名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Name { get; set; }

        /// <summary>
        ///     硬件用户Password
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Password { get; set; }

        /// <summary>
        ///     机器排列顺序
        /// </summary>
        public int SortSeq { get; set; }

        /// <summary>
        ///     硬件UserId
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string UId { get; set; }

        public Guid DmpMachineId { get; set; }

        /// <summary>
        ///     设备类型名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MachineTypeName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [MaxLength(BtlCoreConsts.IPAdressLength)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? TcpPort { get; set; }
    }
}