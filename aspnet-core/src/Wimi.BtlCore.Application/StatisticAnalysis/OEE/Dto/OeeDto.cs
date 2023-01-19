namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;

    public class OeeDto
    {
        public OeeDto()
        {
            this.ShiftDayRanges = new List<NameValueDto>();
            this.MachineOee = new List<MachineOEEDto>();
        }

        public IEnumerable<MachineOEEDto> MachineOee { get; set; }

        public IEnumerable<NameValueDto> ShiftDayRanges { get; set; }
    }
}