using Abp;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Machines
{
    /// <summary>
    ///     设备机器表
    /// </summary>
    [Table("Machines")]
    public class Machine : FullAuditedEntity
    {
        /// <summary>
        ///     机器编号
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        [Comment("设备编号")]
        public string Code { get; set; }

        /// <summary>
        ///     机器描述
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("设备描述")]
        public string Desc { get; set; }

        /// <summary>
        ///     图片报错Id，在表AppBinaryObjects中
        /// </summary>
        [Comment("图片Id(GUID)，在表AppBinaryObjects中")]
        public Guid? ImageId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("弃用")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 产品序列号
        /// </summary>
        [Comment("弃用")]
        public string ProductKey { get; set; }

        [Comment("弃用")]
        public string DeviceSecret { get; set; }

        [Comment("弃用")]
        public bool IsCalLineCapacity { get; set; }


        /// <summary>
        ///     是否可用
        /// </summary>
        [Comment("是否启用")]
        public bool IsActive { get; set; }

        /// <summary>
        ///     设备类型Id
        /// </summary>
        [Comment("设备类型Id")]
        public int MachineTypeId { get; set; }

        /// <summary>
        ///     机器名称
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("设备名称")]
        public string Name { get; set; }

        /// <summary>
        ///     硬件用户Password
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("硬件密码")]
        public string Password { get; set; }

        /// <summary>
        ///     机器排列顺序
        /// </summary>
        [Comment("显示顺序")]
        public int SortSeq { get; set; }

        /// <summary>
        ///     硬件UserId
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("硬件用户名")]
        public string UId { get; set; }

        [Comment("唯一标识(GUID)")]
        public Guid DmpMachineId { get; set; }

        public void SetDmpMachineId()
        {
            this.DmpMachineId = this.DmpMachineId.Equals(Guid.Empty) ? Guid.NewGuid() : this.DmpMachineId;
        }

        /// <summary>
        /// 返回NameValue类型
        /// </summary>
        /// <returns></returns>
        public NameValue ToNameValue()
        {
            return new NameValue(this.ToDescription(), this.Id.ToString());
        }

        /// <summary>
        /// 根据是否存在Desc来拼接所需要的字符串
        /// </summary>
        /// <returns></returns>
        private string ToDescription()
        {
            var codeName = $"{this.Code}-{this.Name}";

            return this.Desc.IsNullOrWhiteSpace() ? codeName : $"{codeName}({this.Desc})";
        }

        public object ProjectTo(IConfigurationProvider autoMapperConfigurationProvider)
        {
            throw new NotImplementedException();
        }
    }
}
