namespace Wimi.BtlCore.Shifts.Dto
{
    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class MachineShiftSolutionInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int? Id { get; set; }

        public string Ids { get; set; }

        public int MachineId { get; set; }

        public string QueryType { get; set; }

        public int? ShiftSolutionId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "MachineId DESC";
            }
        }
    }
}