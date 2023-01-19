namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface
{
    using System.Collections.Generic;

    public interface INotificationTypeManager
    {
        /// <summary>
        /// All notificationType defined in the application.
        /// </summary>
        IDictionary<string, NotificationTypeDefinition> Types { get; }
    }
}