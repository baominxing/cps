namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class MachineToDeviceGroupInputDto
    {
        [Range(1, int.MaxValue)]
        public int DeviceGroupId { get; set; }

        [Range(1, int.MaxValue)]
        public int MachineId { get; set; }
    }
}