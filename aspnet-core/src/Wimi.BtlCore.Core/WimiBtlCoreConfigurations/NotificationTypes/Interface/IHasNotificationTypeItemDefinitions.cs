namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface
{
    using System.Collections.Generic;

    public interface IHasNotificationTypeItemDefinitions
    {
        IList<NotificationTypeDefinition> Items { get; set; }
    }
}