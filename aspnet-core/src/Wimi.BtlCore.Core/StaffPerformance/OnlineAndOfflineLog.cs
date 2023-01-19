using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.StaffPerformance
{
    /// <summary>
    /// 上下线日志实体
    /// </summary>
    [Table("OnlineAndOfflineLogs")]
    public class OnlineAndOfflineLog : Entity<long>
    {
        /// <summary>
        /// 设备号
        /// </summary>
        [Comment("设备key")]
        public int MachineId { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        [Comment("下线时间")]
        public DateTime? OfflineDateTime { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        [Comment("上线时间")]
        public DateTime OnlineDateTime { get; set; }

        /// <summary>
        /// 人员
        /// </summary>
        [Comment("用户Key")]
        public long UserId { get; set; }
    }
}