using System;
using Wimi.BtlCore.BasicData.States;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class StatesRdlcReportDto : State
    {
        public string ShiftDetail_SolutionName { set; get; }
        public string ShiftDetail_StaffShiftName { set; get; }
        public string ShiftDetail_MachineShiftName { set; get; }
        public DateTime ShiftDetail_ShiftDay {set;get;}
    }
}
