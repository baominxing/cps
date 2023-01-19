namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class MachineSettingInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SortSeq asc,Code asc";
            }
        }
    }
}