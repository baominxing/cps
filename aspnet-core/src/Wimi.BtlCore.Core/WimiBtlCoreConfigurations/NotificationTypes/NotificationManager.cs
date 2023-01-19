namespace Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes
{
    using System.Collections.Generic;

    using Abp.Dependency;
    using Abp.Localization;
    using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

    public class NotificationManager : INotificationTypeManager, ISingletonDependency
    {
        private const string Mpa = "Main menu";

        private readonly IWimiBtlCoreConfiguration configuration;

        private readonly IIocResolver iocResolver;

        public NotificationManager(IIocResolver iocResolver, IWimiBtlCoreConfiguration configuration)
        {
            this.iocResolver = iocResolver;

            this.configuration = configuration;

            this.Types = new Dictionary<string, NotificationTypeDefinition>
                             {
                                 {
                                     BtlCoreConsts.NotificationTypesName, 
                                     new NotificationTypeDefinition(
                                     BtlCoreConsts.NotificationTypesName, 
                                     new FixedLocalizableString(Mpa))
                                 }
                             };
        }

        public NotificationTypeDefinition DefaultTypes
        {
            get
            {
                return this.Types[BtlCoreConsts.NotificationTypesName];
            }
        }

        public IDictionary<string, NotificationTypeDefinition> Types { get; }

        public void Initialize()
        {
            var context = new NotificationTypeProviderContext(this);
            foreach (var providerType in this.configuration.NotificationTypeProviders)
            {
                var provider = (NotificationTypeProvider)this.iocResolver.Resolve(providerType);
                provider.SetNotification(context);
            }
        }
    }
}