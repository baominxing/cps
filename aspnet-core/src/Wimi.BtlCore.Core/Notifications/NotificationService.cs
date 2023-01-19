using System.Linq;
using System.Threading.Tasks;

using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;

namespace Wimi.BtlCore.Notifications
{
    public class NotificationService : BtlCoreDomainServiceBase, INotificationService
    {
        private readonly IRepository<NotificationRule> notificationRuleRepository;

        private readonly IRepository<NotificationRuleDetail> notificationRuleDetailRepository;

        public NotificationService(
            IRepository<NotificationRule> notificationRuleRepository,
            IRepository<NotificationRuleDetail> notificationRuleDetailRepository)
        {
            this.notificationRuleRepository = notificationRuleRepository;
            this.notificationRuleDetailRepository = notificationRuleDetailRepository;
        }

        public async Task NotificationRuleNameIsExist(int notificationRuleId, string name)
        {
            var nameIsExist = await Task.FromResult(this.notificationRuleRepository.GetAll().WhereIf(notificationRuleId != 0, c => c.Id != notificationRuleId).Any(s => s.Name == name));

            if (nameIsExist)
            {
                throw new UserFriendlyException(this.L("MessageNotificationNameMustBeUnique"));
            }
        }

        public async Task DeleteNotificationRule(int notificationRuleId)
        {
            await this.notificationRuleRepository.DeleteAsync(notificationRuleId);

            await this.notificationRuleDetailRepository.DeleteAsync(s => s.NotificationRuleId == notificationRuleId);
        }

        public async Task TriggerConditionIsExist(int notificationRuleId, int notificationRuleDetailId, int triggerCondition)
        {
            var triggerConditionIsExist = await Task.FromResult(
                this.notificationRuleDetailRepository.GetAll()
                .WhereIf(notificationRuleDetailId != 0, c => c.Id != notificationRuleDetailId)
                .Any(s => s.NotificationRuleId == notificationRuleId && s.TriggerCondition == triggerCondition));

            if (triggerConditionIsExist)
            {
                throw new UserFriendlyException(this.L("MessageNotificationRuleAlreadyExist", triggerCondition));
            }
        }
    }
}
