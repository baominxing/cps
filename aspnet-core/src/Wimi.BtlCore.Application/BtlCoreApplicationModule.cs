using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BTLMongoDB.Configuration;
using Wimi.BtlCore.BackgroundJobs;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.ThirdpartyApis;

namespace Wimi.BtlCore
{
    [DependsOn(
        typeof(BtlCoreCoreModule),
        typeof(AbpAutoMapperModule), typeof(BackgroudJobModule))]
    public class BtlCoreApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BtlCoreAuthorizationProvider>();

            // Adding custom AutoMapper mappings
            this.Configuration.Modules.AbpAutoMapper().Configurators.Add(mapper => { CustomDtoMapper.CreateMappings(mapper); });

            // 注册微信企业号Token
            //AccessTokenContainer.Register(WeixinYqConfig.CorpId, WeixinYqConfig.CoreSecret);
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(BtlCoreApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AbpMongoDbModuleConfiguration>().Initialize();
            IocManager.Resolve<ThirdpartyApiStore>().Initialize();
        }
    }
}
