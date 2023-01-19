using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    [Table("DeviceGroupYieldMachines")]
    public class DeviceGroupYieldMachine : FullAuditedEntity
    {
        public DeviceGroupYieldMachine()
        {

        }

        public DeviceGroupYieldMachine(int deviceGroupId, int machineId)
        {
            this.MachineId = machineId;
            this.DeviceGroupId = deviceGroupId;
        }

        [Comment("设备组Id")]
        public int DeviceGroupId { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [ForeignKey("DeviceGroupId")]
        [Comment("设备组")]
        public virtual DeviceGroup DeviceGroup { get; set; }

        [ForeignKey("MachineId")]
        [Comment("设备")]
        public virtual Machine Machine { get; set; }
    }
}
