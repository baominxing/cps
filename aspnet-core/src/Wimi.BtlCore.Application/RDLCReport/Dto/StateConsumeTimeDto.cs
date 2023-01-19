using System;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class StateConsumeTimeDto
    {
        public string MachineGroupName { get; set; }
        public string MachineName { get; set; }
        public string Date { get; set; }

        public string ShiftSolutionName { get; set; }

        public string Shift { get; set; }
        public string  RunTime { get; set; }
        public string RunPercent { get; set; }
        public string StopTime { get; set; }
        public string StopPercent { get; set; }
        public string AvaliableTime { get; set; }
        public string AvaliablePercent { get; set; }
        public string  DebugTime { get; set; }
        public string DebugPercent { get; set; }
        public string OfflineTime { get; set; }
        public string OfflinePercent { get; set; }
        public int? MachineShiftDetailId { get; set; }

        public int TotalDuration { get; set; }

        //计算
        public decimal Duration { get; set; }
        public string Code { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
