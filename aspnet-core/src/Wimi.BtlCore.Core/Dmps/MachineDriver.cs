using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Dmps
{
    [Table("MachineDrivers")]
    public class MachineDriver : Entity
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("设备唯一标识(GUID)")]
        public Guid DmpMachineId { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("驱动")]
        public string Driver { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("类型名称")]
        public string TypeName { get; set; }

        [Required]
        [Comment("是否启动")]
        public bool Enable { get; set; }
    }
}
