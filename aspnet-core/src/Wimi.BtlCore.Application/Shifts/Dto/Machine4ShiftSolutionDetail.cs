namespace Wimi.BtlCore.Shifts.Dto
{
    using System;

    public class Machine4ShiftSolutionDetail
    {
        public DateTime EndTime { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }
    }
}