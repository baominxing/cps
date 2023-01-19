namespace Wimi.BtlCore.Weixin.Dto
{
    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.WeChart;

    [AutoMapFrom(typeof(WeChatNotification))]
    public class NotificationTypeUserDto : CreationAuditedEntity
    {
        public bool IsActive { get; set; }

        public string Name { get; set; }

        public long UserId { get; set; }

        public string WeChatId { get; set; }
    }
}