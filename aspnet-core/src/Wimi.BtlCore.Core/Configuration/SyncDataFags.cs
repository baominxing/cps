using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Configuration
{

    [Table("SyncDataFlags")]
    public class SyncDataFlag : CreationAuditedEntity
    {
        // 作业名称
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("作业名称")]
        public string ProcessName { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("上次同步时间")]
        public string LastSyncTime { get; set; }

        [Comment("上次同步数量")]
        public int LastSyncCount { get; set; }

        [Comment("总共同步数量")]
        public int TotalCount { get; set; }
    }
}
