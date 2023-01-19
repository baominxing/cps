using System;

namespace Wimi.BtlCore.BasicData.Machines
{
    public class MachineAlarmStatistices
    {
        public string AlarmCode { get; set; }

        public string AlarmCount { get; set; }

        public string AlarmMessage { get; set; }

        public string MachineGroupName { get; set; }

        public string MachineId { get; set; }

        public string MachineName { get; set; }

        public string MachineShiftSolutionId { get; set; }

        public string MachineShiftSolutionName { get; set; }

        public string ShiftBeginTime { get; set; }

        public string ShiftEndTime { get; set; }

        public string StartTime { get; set; }

        public string StatisticalWay { get; set; }

        public string StatisticalWayString { get; set; }

        public string SummaryDate { get; set; }

        public DateTime ShiftDay { get; set; }

        public int ShiftItemSeq { get; set; }
    }
}