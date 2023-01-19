namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes
{
    using System;
    using System.Collections.Generic;

    using Abp.Localization;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

    public sealed class NotificationTypeDefinition : IHasNotificationTypeItemDefinitions
    {
        public NotificationTypeDefinition(string name, ILocalizableString displayName, NotificationTriggerMode triggerMode = NotificationTriggerMode.ByTime)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "MenuNameCannotBeNull"));
            }

            if (displayName == null)
            {
                throw new ArgumentNullException(nameof(displayName), LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "MenuDisplayNameCannotBeNull"));
            }

            this.Name = name;
            this.DisplayName = displayName;
            this.TriggerMode = triggerMode;

            this.Items = new List<NotificationTypeDefinition>();
        }
          
        public ILocalizableString DisplayName { get; set; }

        public IList<NotificationTypeDefinition> Items { get; set; }

        public string Name { get; set; }

        public NotificationTriggerMode TriggerMode { get; set; }

        public NotificationTypeDefinition AddItem(NotificationTypeDefinition item)
        {
            this.Items.Add(item);
            return this;
        }
    }
}