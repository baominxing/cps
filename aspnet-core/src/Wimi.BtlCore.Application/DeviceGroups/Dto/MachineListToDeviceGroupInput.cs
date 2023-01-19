namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class MachineListToDeviceGroupInputDto
    {
        [Range(1, int.MaxValue)]
        public int DeviceGroupId { get; set; }

        public List<int> MachineIdList { get; set; }
    }
}