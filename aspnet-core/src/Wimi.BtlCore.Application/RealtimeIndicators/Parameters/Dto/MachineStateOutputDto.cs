namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.Dto;

    public class MachineStateOutputDto
    {
        public MachineStateOutputDto()
        {
            this.Items = new List<MachineStatusListDto>();
        }

        public List<MachineStatusListDto> Items { get; set; }

        public int TotalCount { get; set; }
    }
}