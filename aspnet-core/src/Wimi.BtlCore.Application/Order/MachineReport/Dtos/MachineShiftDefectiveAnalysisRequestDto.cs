using System;

namespace Wimi.BtlCore.Order.MachineReport.Dtos
{
    public class MachineShiftDefectiveAnalysisRequestDto
    {
        public int MachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; }
    }
}
