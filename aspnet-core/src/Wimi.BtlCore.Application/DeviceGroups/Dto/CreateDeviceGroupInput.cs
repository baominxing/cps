namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using System.ComponentModel.DataAnnotations;
    using Wimi.BtlCore.BasicData.DeviceGroups;

    public class CreateDeviceGroupInputDto
    {
        [Required]
        [StringLength(DeviceGroup.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public int? ParentId { get; set; }

        public int Seq { get; set; }
    }
}