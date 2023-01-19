namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class MoveDeviceGroupInputDto
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int? NewParentId { get; set; }
    }
}