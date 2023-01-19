namespace Wimi.BtlCore.Notifications
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum EnumMessageType
    {
        /// <summary>
        /// 设备报警
        /// </summary>
        [Display(Name = "设备报警")]
        DeviceAlarm = 0,

        /// <summary>
        /// 刀具提醒
        /// </summary>
        [Display(Name = "刀具提醒")]
        ToolReminder = 1,

        /// <summary>
        /// 产量统计
        /// </summary>
        [Display(Name = "产量统计")]
        YieldStatistics = 2,
    }
}
