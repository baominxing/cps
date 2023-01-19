using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes
{

    public class NotificationTypeProviderContext : INotificationTypeProviderContext
    {
        public NotificationTypeProviderContext(INotificationTypeManager manager)
        {
            this.Manager = manager;
        }

        public INotificationTypeManager Manager { get; }
    }
}