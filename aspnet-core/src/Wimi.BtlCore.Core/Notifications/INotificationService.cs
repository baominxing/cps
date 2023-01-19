namespace Wimi.BtlCore.Notifications
{
    using System.Threading.Tasks;

    using Abp.Domain.Services;

    public interface INotificationService : IDomainService
    {
        Task NotificationRuleNameIsExist(int notificationRuleId, string name);

        Task DeleteNotificationRule(int notificationRuleId);

        Task TriggerConditionIsExist(int notificationRuleId, int notificationRuleDetailId, int triggerCondition);
    }
}