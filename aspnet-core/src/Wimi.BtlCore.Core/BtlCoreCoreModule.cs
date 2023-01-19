using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BTLMongoDB;
using Wimi.BtlCore.BTLMongoDB.Configuration;
using Wimi.BtlCore.BTLMongoDB.Repositories;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Features;
using Wimi.BtlCore.Localization;
using Wimi.BtlCore.MultiTenancy;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.Runtime.Exception;
using Wimi.BtlCore.ThirdpartyApis;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;
using Wimi.BtlCore.Timing;
using Wimi.BtlCore.WimiBtlCoreConfigurations;
using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes;
using Wimi.BtlCore.WimiBtlCoreConfigurations.NotificationTypes.Interface;

namespace Wimi.BtlCore
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class BtlCoreCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<INotificationTypeManager, NotificationManager>();
            IocManager.RegisterIfNot<IWimiBtlCoreConfiguration, WimiBtlCoreConfiguration>();
            IocManager.RegisterIfNot<IThirdpartyApiStore, ThirdpartyApiStore>();
            IocManager.RegisterIfNot<IAbpMongoDbModuleConfiguration, AbpMongoDbModuleConfiguration>();
            IocManager.RegisterIfNot<IMongoDatabaseProvider, MongoDatabaseProvider>();
            IocManager.RegisterIfNot(typeof(IMongoRepository<>), typeof(MongoDbRepositoryBase<>));

            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            BtlCoreLocalizationConfigurer.Configure(Configuration.Localization);

            // Adding feature providers
            Configuration.Features.Providers.Add<BtlCoreFeatureProvider>();

            // Adding setting providers
            Configuration.Settings.Providers.Add<AppSettingProvider>();

            // Adding notification providers
            Configuration.Notifications.Providers.Add<AppNotificationProvider>();


            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = BtlCoreConsts.MultiTenancyEnabled;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;

            //默认关闭审计日志
            this.Configuration.Auditing.IsEnabled = false;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            //Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(BtlCoreCoreModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
            IocManager.Resolve<NotificationManager>().Initialize();
            IocManager.Resolve<InfluxdbManager>().Initialize();
        }
    }
}
