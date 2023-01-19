namespace Wimi.BtlCore.Shifts.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.Shifts;

    public class ShiftInfoInputDto
    {
        public int ShiftSolutionId { get; set; }

        public List<ShiftSolutionItem> ShiftSolutionItems { get; set; }
    }
}