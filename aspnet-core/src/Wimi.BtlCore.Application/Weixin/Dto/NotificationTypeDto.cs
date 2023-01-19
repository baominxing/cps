namespace Wimi.BtlCore.Weixin.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.WeChart;

    [AutoMapFrom(typeof(NotificationType))]
    public class NotificationTypeDto : CreationAuditedEntityDto
    {
        public string DisplayName { get; set; }

        public int MemberCount { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}