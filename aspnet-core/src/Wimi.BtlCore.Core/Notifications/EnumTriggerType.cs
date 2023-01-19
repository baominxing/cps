namespace Wimi.BtlCore.Notifications
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 消息触发类型
    /// </summary>
    public enum EnumTriggerType
    {
        /// <summary>
        /// 按时间触发
        /// </summary>
        [Display(Name = "按时间触发")]
        TriggerWithTime = 0,

        /// <summary>
        /// 按次数触发
        /// </summary>
        [Display(Name = "按次数触发")]
        TriggerWithCount = 1,

        /// <summary>
        /// 按班次结束触发
        /// </summary>
        [Display(Name = "按班次结束触发")]
        TriggerWithShift = 2
    }
}
