namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class GetGatherParamsInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string MachineCode { get; set; }

        public long MachineId { get; set; }

        public string Name { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SortSeq,Code,Name";
            }
        }
    }
}