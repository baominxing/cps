namespace Wimi.BtlCore.Notifications
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 通知方式（微信，邮件）
    /// </summary>
    public enum EnumNotificationType
    {
        /// <summary>
        /// 微信
        /// </summary>
        [Display(Name = "微信")]
        WeChat = 0,

        /// <summary>
        /// 邮件
        /// </summary>
        [Display(Name = "邮件")]
        Email = 1
    }
}
