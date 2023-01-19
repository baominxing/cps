using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Dmps
{
    [Table("DmpMachines")]
    public class DmpMachine : CreationAuditedEntity
    {
        public DmpMachine()
        {

        }

        public DmpMachine(int machineId,int dmpId)
        {
            MachineId = machineId;
            DmpId = dmpId;
        }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("DMP Id")]
        public int DmpId { get; set; }
    }
}
