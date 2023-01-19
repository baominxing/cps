using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.DefectiveParts
{
    [Table("DefectivePartReasons")]
    public class DefectivePartReason : CreationAuditedEntity
    {
        public DefectivePartReason()
        {

        }

        public DefectivePartReason(int reasonId, int partId)
        {
            this.ReasonId = reasonId;
            this.PartId = partId;
        }
        public DefectivePartReason(int reasonId, int partId, string partCode)
        {
            this.ReasonId = reasonId;
            this.PartId = partId;
            this.PartCode = partCode;
        }

        [Comment("不良原因ID")]
        public int ReasonId { get; set; }

        [Comment("不良部位ID")]
        public int PartId { get; set; }

        [Comment("不良部位编号")]
        public string PartCode { get; set; }
    }
}
