using System;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class ReportInputDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string ReportName { get; set; }
        public int ShiftSolutionId { get; set; }

       
    }
}
