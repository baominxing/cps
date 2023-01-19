using System;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    /// <summary>
    /// 某台设备当天的状态相关信息
    /// </summary>
    public class MachineStates4YieldAnalysisDto
    {
        /// <summary>
        /// 状态结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 班次Id
        /// </summary>
        public int ShiftId { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }

        // 状态开始时间
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public string StateCode { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// 操作工Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 操作人名
        /// </summary>
        public string UserName { get; set; }
    }
}
