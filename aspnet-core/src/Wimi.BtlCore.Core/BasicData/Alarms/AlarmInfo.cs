using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.Alarms
{
    [Table("AlarmInfos")]
    public class AlarmInfo : CreationAuditedEntity<long>
    {
        /// <summary>
        /// 报警编号
        /// </summary>
        [Comment("报警编号")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        /// <summary>
        /// 报警内容
        /// </summary>
        [Comment("报警内容")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 4)]
        public string Message { get; set; }

        /// <summary>
        /// 报警原因
        /// </summary>
        [Comment("报警原因")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 4)]
        public string Reason { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Comment("设备Id")]
        public int MachineId { get; set; }
    }
}
