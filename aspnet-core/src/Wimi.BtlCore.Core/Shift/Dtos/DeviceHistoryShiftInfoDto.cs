using System.Collections.Generic;

namespace Wimi.BtlCore.Shift.Dtos
{
    public class DeviceHistoryShiftInfoDto
    {
        public int DeviceId { get; set; }

        public string EndDate { get; set; }

        public bool IsHistory { get; set; }

        public List<ShiftInfoDto2> ShiftInfoDtos { get; set; }

        public string ShiftSolutionName { get; set; }

        public string StartDate { get; set; }
    }
}
