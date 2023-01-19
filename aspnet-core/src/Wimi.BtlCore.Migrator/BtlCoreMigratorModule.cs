using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Wimi.BtlCore.EntityFrameworkCore;
using Wimi.BtlCore.Migrator.DependencyInjection;
using Abp.Dependency;
using System;
using AgileConfig.Client;
using Microsoft.AspNetCore.Hosting;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Migrator
{
    [DependsOn(typeof(BtlCoreEntityFrameworkModule))]
    public class BtlCoreMigratorModule : AbpModule
    {
        public BtlCoreMigratorModule(BtlCoreEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            #region ∂¡»°≈‰÷√÷––ƒ
            var configClient = new ConfigClient();

            if (configClient != null && configClient.Status == ConnectStatus.Disconnected)
            {
                configClient.ConnectAsync().Wait(5000);

                var configuration = new ConfigurationBuilder()
                    .AddAgileConfig(configClient)
                    .SetBasePath(Environment.CurrentDirectory)
                    .Build();

                IocManager.IocContainer.Register(Component.For<IConfiguration>().Instance(configuration));
            }
            #endregion

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(typeof(IEventBus), () => IocManager.IocContainer.Register(Component.For<IEventBus>().Instance(NullEventBus.Instance)));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BtlCoreMigratorModule).GetAssembly());
            IocManager.RegisterIfNot<IConfiguration, MigrationConfiguration>();
            ServiceCollectionRegistrar.Register(IocManager);
        }

        public override void PostInitialize()
        {
            Configuration.DefaultNameOrConnectionString = AppSettings.Database.ConnectionString;
        }
    }
}
