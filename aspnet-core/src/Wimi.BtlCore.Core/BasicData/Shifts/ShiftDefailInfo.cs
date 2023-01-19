using Microsoft.EntityFrameworkCore;
using System;

namespace Wimi.BtlCore.BasicData.Shifts
{
    [Owned]
    public class ShiftDefailInfo
    {
        [Comment("班次方案名称")]
        public string SolutionName { get; set; }

        [Comment("班次日期")]
        public DateTime? ShiftDay { get; set; }

        [Comment("设备班次名称")]
        public string MachineShiftName { get; set; }

        [Comment("人员班次名称")]
        public string StaffShiftName { get; set; }
    }
}
