namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System.Collections.Generic;

    using Abp.AutoMapper;

    [AutoMap(typeof(StaffPerformance.StaffPerformance))]
    public class StaffPerformanceDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public List<StaffPerformanceStateRateDto> ReasonRates { get; set; }

        public List<StaffPerformanceStateRateDto> StateRates { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}