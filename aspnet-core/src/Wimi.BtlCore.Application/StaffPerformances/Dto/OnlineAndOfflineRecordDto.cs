namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;

    public class OnlineAndOfflineRecordDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 下线时间
        /// </summary>
        public DateTime? OfflineTime { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime OnlineTime { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string UserName { get; set; }
    }
}