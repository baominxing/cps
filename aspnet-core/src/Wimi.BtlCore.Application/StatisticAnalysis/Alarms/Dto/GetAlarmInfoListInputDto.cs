namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class GetAlarmInfoListInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Code { get; set; }

        public string Content { get; set; }

        public string Filter { get; set; }

        public string Reason { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(this.Sorting))
            {
                this.Sorting = "SortSeq,MachineCode";
            }
        }
    }
}