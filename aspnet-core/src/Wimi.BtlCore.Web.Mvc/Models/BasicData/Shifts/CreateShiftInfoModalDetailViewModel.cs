using System;

namespace Wimi.BtlCore.Web.Models.BasicData.Shifts
{
    public class CreateShiftInfoModalDetailViewModel
    {
        public DateTime CreationTime { get; set; }

        public decimal Duration { get; set; }

        public DateTime EndTime { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public bool IsNextDay { get; set; }
    }
}
