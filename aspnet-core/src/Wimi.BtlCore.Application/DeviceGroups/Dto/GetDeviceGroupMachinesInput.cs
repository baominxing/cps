namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using Abp.Runtime.Validation;
    using System.ComponentModel.DataAnnotations;
    using Wimi.BtlCore.Dto;

    public class GetDeviceGroupMachinesInputDto : PagedAndSortedInputDto, IShouldNormalize
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int SortSeq { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SortSeq,Code";
            }
        }
    }
}