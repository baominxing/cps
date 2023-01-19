using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Wimi.BtlCore.Configuration;
using Abp.IO;
using Hangfire.Logging;
using System;

namespace Wimi.BtlCore.Web.Startup
{
    [DependsOn(typeof(BtlCoreWebCoreModule))]
    public class BtlCoreWebMvcModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public BtlCoreWebMvcModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<BtlCoreNavigationProvider>();

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BtlCoreWebMvcModule).GetAssembly());

        }

        public override void PostInitialize()
        {
            string webRootPath = _env.WebRootPath;
            var appFolders = this.IocManager.Resolve<BtlFolders>();

            appFolders.SampleProfileImagesFolder =$"{webRootPath}/Content/Images/SampleProfilePics";
            appFolders.TempFileDownloadFolder = $"{webRootPath}/Temp/Downloads";
            appFolders.TempFileUploadFolder = $"{webRootPath}/{AppPath.TempFileUpload}";

            appFolders.IcosFolder = $"{webRootPath}/{AppPath.Icos}";

            appFolders.WebLogsFolder = $"{webRootPath}/Logs";
            appFolders.ConfigurationsFloder = $"{webRootPath}/Configurations";
            appFolders.VisualImgFloder = $"{webRootPath}/{AppPath.VisualImage}";
            appFolders.ComponentConfigFolder = $"{webRootPath}/Configurations/ComponentConfig";

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.TempFileUploadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.IcosFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.ComponentConfigFolder);
            }
            catch
            {
                // ignored
            }
        }

        public class CustomLogProvider : ILogProvider
        {
            public ILog GetLogger(string name)
            {
                return new CustomLogger { Name = name };
            }
        }

        public class CustomLogger : ILog
        {
            public string Name { get; set; }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (messageFunc == null)
                {
                    return logLevel > LogLevel.Debug;
                }
                return true;
            }
        }
    }
}
