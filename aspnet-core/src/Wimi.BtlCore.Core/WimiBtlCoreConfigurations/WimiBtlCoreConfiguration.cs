namespace Wimi.BtlCore.WimiBtlCoreConfigurations
{
    using Abp.Collections;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes;

    internal class WimiBtlCoreConfiguration : IWimiBtlCoreConfiguration
    {
        public WimiBtlCoreConfiguration()
        {
            this.NotificationTypeProviders = new TypeList<NotificationTypeProvider>();
        }

        public ITypeList<NotificationTypeProvider> NotificationTypeProviders { get; private set; }
    }
}