using System.Collections.Generic;
using Wimi.BtlCore.Dashboard.Dtos;

namespace Wimi.BtlCore.MultiTenancy.Dashboard.Dto
{
    public class GetMachineUsedTimeRateDto
    {
        public GetMachineUsedTimeRateDto()
        {
            this.CurrentShiftMachineUsedTimeRates = new List<MachineUsedTimeRateDto>();
        }

        public List<MachineUsedTimeRateDto> CurrentShiftMachineUsedTimeRates { get; set; }
    }
}