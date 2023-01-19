using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Dmps
{
    [Table("DriverConfigs")]
    public class DriverConfig : Entity
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("设备唯一标识(GUID)")]
        public Guid DmpMachineId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("编号")]
        public string Code { get; set; }

        [Comment("值")]
        public string Value { get; set; }
    }
}
