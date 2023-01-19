using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Order.MachineProcesses
{
    [Table("MachineProcesses")]
    public class MachineProcess : FullAuditedEntity
    {
        [Comment("设备key")]
        [Required]
        public int MachineId { get; set; }

        [Comment("工序key")]
        [Required]
        public int ProcessId { get; set; }

        [Comment("产品key")]
        [Required]
        public int ProductId { get; set; }

        [Comment("更改产品的用户key")]
        public int? ChangeProductUserId { get; set; }

        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }
    }
}
