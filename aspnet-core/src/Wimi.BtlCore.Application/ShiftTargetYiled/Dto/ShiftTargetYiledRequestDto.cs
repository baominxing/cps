namespace Wimi.BtlCore.ShiftTargetYiled.Dto
{
    using System;

    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class ShiftTargetYiledRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime? EndTime { get; set; }

        public string ProductInput { get; set; }

        public string ShiftName { get; set; }

        public DateTime? StartTime { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting)) this.Sorting = "shiftDay desc,ProductId,ShiftSolutionItemId";
        }
    }
}