using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Dmps
{
    [Table("Dmps")]
    public class Dmp : CreationAuditedEntity
    {
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("名称")]
        public string Name { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("服务编号")]
        public string ServiceCode { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("端口")]
        public string WebPort { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("IP地址")]
        public string IpAdress { get; set; }
    }
}
