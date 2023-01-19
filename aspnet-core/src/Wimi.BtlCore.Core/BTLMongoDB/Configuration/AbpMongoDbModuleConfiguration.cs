using Abp.Configuration;
using Microsoft.Extensions.Configuration;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BTLMongoDB.Configuration
{
    public class AbpMongoDbModuleConfiguration : IAbpMongoDbModuleConfiguration
    {
        private readonly ISettingManager _settingManager;

        public AbpMongoDbModuleConfiguration(ISettingManager iSettingManager)
        {
            _settingManager = iSettingManager;
        }

        public string ConnectionString { get; set; }

        public string DatatabaseName { get; set; }

        public void Initialize()
        {
            this.ConnectionString = AppSettings.MongodbDatabase.ConnectionString;
            this.DatatabaseName = AppSettings.MongodbDatabase.DatabaseName;
        }
    }
}