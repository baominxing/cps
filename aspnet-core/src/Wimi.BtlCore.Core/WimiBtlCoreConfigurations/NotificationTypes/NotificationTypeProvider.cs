namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes
{
    using Abp.Dependency;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

    public abstract class NotificationTypeProvider : ISingletonDependency
    {
        public abstract void SetNotification(INotificationTypeProviderContext context);
    }
}