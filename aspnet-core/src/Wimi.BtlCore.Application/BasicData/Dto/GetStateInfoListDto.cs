namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Dto;

    public class GetStateInfoListDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public string Filter { get; set; }

        public string Hexcode { get; set; }

        public bool IsPlaned { get; set; }

        public EnumMachineStateType Type { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}