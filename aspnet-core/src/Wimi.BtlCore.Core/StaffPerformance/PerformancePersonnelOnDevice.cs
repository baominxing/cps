using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.StaffPerformance
{
    [Table("PerformancePersonnelOnDevices")]
    public class PerformancePersonnelOnDevice : Entity
    {
        [Comment("用户Key")]
        public int MachineId { get; set; }

        [Comment("设备key")]
        public DateTime OnlineDate { get; set; }

        [Comment("上线时间")]
        public long UserId { get; set; }
    }
}