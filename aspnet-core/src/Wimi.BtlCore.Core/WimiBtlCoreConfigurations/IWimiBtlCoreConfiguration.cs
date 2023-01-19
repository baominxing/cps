namespace Wimi.BtlCore.WimiBtlCoreConfigurations
{
    using Abp.Collections;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes;

    public interface IWimiBtlCoreConfiguration
    {
        ITypeList<NotificationTypeProvider> NotificationTypeProviders { get; }
    }
}