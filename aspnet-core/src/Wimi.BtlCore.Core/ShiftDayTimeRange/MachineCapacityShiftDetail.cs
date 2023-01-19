using System;

namespace Wimi.BtlCore.ShiftDayTimeRange
{
    public class MachineCapacityShiftDetail
    {
        /// <summary>
        /// 班次信息ID
        /// </summary>
        public int MachineShiftDetailId { get; set; }

        /// <summary>
        /// SolutionName
        /// </summary>
        public string ShiftDetail_SolutionName { get; set; }

        /// <summary>
        /// 设备班次名称
        /// </summary>
        public string ShiftDetail_MachineShiftName { get; set; }

        /// <summary>
        /// 工厂日
        /// </summary>
        public DateTime? ShiftDetail_ShiftDay { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
