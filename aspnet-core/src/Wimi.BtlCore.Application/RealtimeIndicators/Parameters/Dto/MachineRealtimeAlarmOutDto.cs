namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using System.Collections.Generic;

    public class MachineRealtimeAlarmOutDto
    {
        public MachineRealtimeAlarmOutDto()
        {
            this.Items = new List<GetMachineAlarmOutputDto>();
        }

        public List<GetMachineAlarmOutputDto> Items { get; set; }

        public int TotalCount { get; set; }
    }
}