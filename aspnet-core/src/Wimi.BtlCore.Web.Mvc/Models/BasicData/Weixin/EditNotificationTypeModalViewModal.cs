namespace Wimi.BtlCore.Web.Models.BasicData.Weixin
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.WeChart;

    [AutoMapFrom(typeof(NotificationType))]
    public class EditNotificationTypeModalViewModel
    {
        public string DisplayName { get; set; }

        public long? Id { get; set; }
    }
}
