using System;
using Abp.AutoMapper;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;

namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    [AutoMap(typeof(CorrectQueryDateDto))]
    public class ShiftCalendarDto
    {
        public string ShiftId { get; set; }

        public string ShiftName { get; set; }

        public string ShiftDay { get; set; }

        public string ShiftWeek { get; set; }

        public string ShiftMonth { get; set; }

        public string ShiftYear { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

    }
}
