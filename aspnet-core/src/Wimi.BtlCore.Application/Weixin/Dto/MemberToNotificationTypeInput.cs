namespace Wimi.BtlCore.Weixin.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class MemberToNotificationTypeInputDto
    {
        [Range(1, int.MaxValue)]
        public int NotificationTypeId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}