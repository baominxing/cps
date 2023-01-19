using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.ThirdpartyApis
{
    [Table("ThirdpartyApis")]
    public class ThirdpartyApi : FullAuditedEntity<Guid>
    {
        [Comment("编码")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        [Comment("名字")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Name { get; set; }

        [Comment("地址")]
        [Required]
        [MaxLength(500)]
        public string Url { get; set; }

        [Comment("类型")]
        public EnumApiType Type { get; set; }
    }
}