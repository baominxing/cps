using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    [Table("MachineDeviceGroups")]
    public class MachineDeviceGroup : CreationAuditedEntity
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MachineDeviceGroup" /> class.
        /// </summary>
        public MachineDeviceGroup()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MachineDeviceGroup" /> class.
        /// </summary>
        /// <param name="machineId">Id of the Machine.</param>
        /// <param name="deviceGroupId">Id of the <see cref="DeviceGroup" />.</param>
        public MachineDeviceGroup(int machineId, int deviceGroupId)
        {
            this.MachineId = machineId;
            this.DeviceGroupId = deviceGroupId;
        }

        public MachineDeviceGroup(int machineId, int deviceGroupId, string deviceGroupCode)
        {
            this.MachineId = machineId;
            this.DeviceGroupId = deviceGroupId;
            this.DeviceGroupCode = deviceGroupCode;
        }

        /// <summary>
        ///     Id of the <see cref="DeviceGroup" />.
        /// </summary>
        [Comment("设备组Id")]
        public virtual int DeviceGroupId { get; set; }

        [StringLength(128)]
        [Comment("设备组编号")]
        public virtual string DeviceGroupCode { get; set; }

        /// <summary>
        ///     Id of the Machine.
        /// </summary>
        [Comment("设备Id")]
        public virtual int MachineId { get; set; }
    }
}
