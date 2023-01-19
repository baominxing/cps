namespace Wimi.BtlCore.OEE
{
    using System;
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.CommonEnums;

    public class OeeAnalysis
    {
        public OeeAnalysis()
        {
            this.ShiftDayRanges = new List<NameValueDto>();
        }

        public int MachineId { get; set; }

        public IEnumerable<int> MachineIdList { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string ShiftDay { get; set; }

        public string SummaryDate { get; set; }

        public EnumStatisticalWays StatisticalWays { get; set; }

        public IEnumerable<NameValueDto> ShiftDayRanges { get; set; }
    }
}