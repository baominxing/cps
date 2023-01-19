namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class MachineTypeInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Name";
            }
        }
    }
}