namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("NotificationRules")]
    public class NotificationRule : FullAuditedEntity
    {
        [Comment("设备组Ids")]
        [MaxLength(100)]
        public string DeviceGroupIds { get; set; }

        [Comment("规则名称")]
        [MaxLength(40)]
        public string Name { get; set; }

        [Comment("消息类型")]
        public EnumMessageType MessageType { get; set; }

        [Comment("触发类型")]
        public EnumTriggerType TriggerType { get; set; }

        /// <summary>
        /// 获取设备组Ids的集合形式
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDeviceGroupIds()
        {
            return this.DeviceGroupIds.Split(',');
        }
    }
}
