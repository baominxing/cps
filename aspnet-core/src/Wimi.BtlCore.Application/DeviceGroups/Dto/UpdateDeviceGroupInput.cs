namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using System.ComponentModel.DataAnnotations;
    using Wimi.BtlCore.BasicData.DeviceGroups;

    public class UpdateDeviceGroupInputDto
    {
        [Required]
        [StringLength(DeviceGroup.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int Seq { get; set; }
    }
}