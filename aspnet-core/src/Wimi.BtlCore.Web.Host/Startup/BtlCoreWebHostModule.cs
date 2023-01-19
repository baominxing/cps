using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Web.Host.Startup
{
    [DependsOn(
       typeof(BtlCoreWebCoreModule))]
    public class BtlCoreWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;


        public BtlCoreWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BtlCoreWebHostModule).GetAssembly());
            this.Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }
    }
}
