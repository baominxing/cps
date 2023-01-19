namespace Wimi.BtlCore.WeChart
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp;
    using Abp.Collections.Extensions;
    using Abp.Domain.Repositories;
    using Abp.Domain.Services;
    using Abp.Domain.Uow;
    using Abp.UI;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

    public class NotificationTypeManager : DomainService
    {
        private readonly INotificationTypeManager notificationTypeManager;

        public NotificationTypeManager(
            IRepository<NotificationType> notificationTypeRepository, 
            INotificationTypeManager notificationTypeManager)
        {
            this.NotificationTypeRepository = notificationTypeRepository;
            this.notificationTypeManager = notificationTypeManager;
        }

        protected IRepository<NotificationType> NotificationTypeRepository { get; }

        [UnitOfWork]
        public virtual async Task CreateAsync(NotificationType notificationType)
        {
            notificationType.Name = await this.GetNextChildCodeAsync(notificationType.ParentId);
            await this.ValidateNotificationTypeAsync(notificationType);
            await this.NotificationTypeRepository.InsertAsync(notificationType);
        }

        public async Task<List<NotificationType>> FindChildrenAsync(int? parentId, bool recursive = false)
        {
            if (recursive)
            {
                if (!parentId.HasValue)
                {
                    return await this.NotificationTypeRepository.GetAllListAsync();
                }

                var code = await this.GetCodeAsync(parentId.Value);
                return
                    await
                    this.NotificationTypeRepository.GetAllListAsync(
                        ou => ou.Name.StartsWith(code) && ou.Id != parentId.Value);
            }

            return await this.NotificationTypeRepository.GetAllListAsync(ou => ou.ParentId == parentId);
        }

        public virtual async Task<string> GetCodeAsync(int id)
        {
            return (await this.NotificationTypeRepository.GetAsync(id)).Name;
        }

        public virtual async Task<NotificationType> GetLastChildOrNullAsync(int? parentId)
        {
            var children = await this.NotificationTypeRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            return children.OrderBy(c => c.Name).LastOrDefault();
        }

        public virtual async Task<string> GetNextChildCodeAsync(int? parentId)
        {
            var lastChild = await this.GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await this.GetCodeAsync(parentId.Value) : null;
                return DeviceGroup.AppendCode(parentCode, DeviceGroup.CreateCode(1));
            }

            return DeviceGroup.CalculateNextCode(lastChild.Name);
        }

        public IEnumerable<NotificationTypeDefinition> NotificationTypeList(string typeName)
        {
            var typeDefinition = this.notificationTypeManager.Types.GetOrDefault(typeName);
            if (typeDefinition == null)
            {
                throw new AbpException(this.L("NoNotificationTypeWithGivenName{0}", typeName));
            }

            return typeDefinition.Items;
        }

        public virtual async Task ValidateNotificationTypeAsync(NotificationType notificationType)
        {
            var siblings =
                (await this.FindChildrenAsync(notificationType.ParentId)).Where(ou => ou.Id != notificationType.Id)
                    .ToList();

            if (siblings.Any(ou => ou.DisplayName == notificationType.DisplayName))
            {
                throw new UserFriendlyException(this.L("NotificationTypeAlreadyExist{0}", notificationType.DisplayName));
            }
        }
    }
}