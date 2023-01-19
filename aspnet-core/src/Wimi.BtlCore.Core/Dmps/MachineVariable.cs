using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Dmps
{


    [Table("MachineVariables")]
    public class MachineVariable : Entity<Guid>
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("设备唯一标识(GUID)")]
        public Guid DmpMachineId { get; set; }

        [Comment("类型")]
        public int Type { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("编号")]
        public string Code { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("描述")]
        public string Description { get; set; }

        [Required]
        [Comment("设备地址")]
        public string DeviceAddress { get; set; }

        [Comment("存取")]
        public int Access { get; set; }

        [Comment("数据长度")]
        public int DataLength { get; set; }

        [Comment("值类型")]
        public double ValueFactor { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("默认值")]
        public string DefaultValue { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("数据类型")]
        public string DataType { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("方法参数")]
        public string Methodparam { get; set; }

        [Comment("顺序")]
        public int Index { get; set; }
    }
}
