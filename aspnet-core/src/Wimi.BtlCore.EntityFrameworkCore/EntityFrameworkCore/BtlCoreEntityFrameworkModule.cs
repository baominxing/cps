using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using System;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.EntityFrameworkCore.Seed;

namespace Wimi.BtlCore.EntityFrameworkCore
{
    [DependsOn(
        typeof(BtlCoreCoreModule),
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class BtlCoreEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                try
                {
                    Configuration.Modules.AbpEfCore().AddDbContext<BtlCoreDbContext>(options =>
                    {
                        if (options.ExistingConnection != null)
                        {
                            BtlCoreDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                        }
                        else
                        {
                            BtlCoreDbContextConfigurer.Configure(options.DbContextOptions, AppSettings.Database.ConnectionString);
                        }
                    });
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }


            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BtlCoreEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }

    }
}
