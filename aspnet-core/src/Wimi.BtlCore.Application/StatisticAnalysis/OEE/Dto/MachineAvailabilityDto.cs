namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.AutoMapper;
    using Wimi.BtlCore.OEE;

    [AutoMap(typeof(OeeResponse))]
    public class MachineAvailabilityDto
    {
        public MachineAvailabilityDto()
        {
            this.UnplannedPauses = new List<UnplannedPauseDto>();
        }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal PlannedWorkTime { get; set; }

        public decimal ActualWorkTime { get; set; }

        public string ShiftDay { get; set; }

        public decimal Rate => this.PlannedWorkTime != 0 ? Math.Round(this.ActualWorkTime / this.PlannedWorkTime, 4)  : 1;

        public IEnumerable<UnplannedPauseDto> UnplannedPauses { get; set; }
    }
}