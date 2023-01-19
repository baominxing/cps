namespace Wimi.BtlCore.Notifications
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumNotificationRecordStatus
    {
        /// <summary>
        /// 已发送
        /// </summary>
        [Display(Name = "已发送")]
        Undispatched = 0,

        /// <summary>
        /// 未发送
        /// </summary>
        [Display(Name = "未发送")]
        Dispatched = 1,
    }
}
