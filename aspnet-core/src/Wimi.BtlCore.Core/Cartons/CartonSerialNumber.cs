using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Cartons
{
    [Table("CartonSerialNumbers")]
    public class CartonSerialNumber : AuditedEntity<long>
    {
        [Comment("序列号")]
        public string SerialNumber { get; set; }

        [Comment("状态")]
        public SerialNumberStatus Status { get; set; }

        [Comment("日期 20230106")]
        public int DateKey { get; set; }
    }

    public enum SerialNumberStatus
    {
        New = 0,
        Appointment = 1,
        Complete = 2
    }
}
