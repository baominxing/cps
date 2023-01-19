namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System.Collections.Generic;

    public class ProductionChartRequestDto
    {
        public ProductionChartRequestDto()
        {
            this.DateRange = new DateRangeRequestDto();
        }

        public string ByUserOrMachine { get; set; }

        public DateRangeRequestDto DateRange { get; set; }

        public int MachineId { get; set; }

        public List<long> MachineIds { get; set; }

        public long? UserId { get; set; }

        public int ShiftSolutionId { get; set; }

        public string ShiftSolutionName { get; set; }
    }
}