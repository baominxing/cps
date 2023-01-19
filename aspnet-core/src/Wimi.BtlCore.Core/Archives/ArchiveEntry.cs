using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Archives
{
    [Table("ArchiveEntries")]
    public class ArchiveEntry : AuditedEntity
    {
        [Comment("归档源表表名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string TargetTable { get; set; }

        [Comment("归档目标表名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ArchivedTable { get; set; }

        [Comment("归档列名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ArchiveColumn { get; set; }

        [Comment("归档列内容")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ArchiveValue { get; set; }

        [Comment("归档最后一次数据数量")]
        public long ArchiveCount { get; set; }

        [Comment("归档总计数量")]
        public long ArchiveTotalCount { get; set; }

        [Comment("归档结果信息")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 10)]
        public string ArchivedMessage { get; set; }
    }
}
