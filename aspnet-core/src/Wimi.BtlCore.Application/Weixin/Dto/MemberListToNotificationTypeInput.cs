namespace Wimi.BtlCore.Weixin.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class MemberListToNotificationTypeInputDto
    {
        public List<int> MemberIdList { get; set; }

        [Range(1, int.MaxValue)]
        public int NotificationTypeId { get; set; }
    }
}