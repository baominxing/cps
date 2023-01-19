using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Migrator
{
    public class MigrationConfiguration : IConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;
        public MigrationConfiguration()
        {
            AppConfigurations.ClearCache();

            _appConfiguration = AppConfigurations.Get(typeof(BtlCoreMigratorModule).GetAssembly().GetDirectoryPathOrNull());
        }

        string IConfiguration.this[string key] { get => _appConfiguration.GetValue<string>(key); set => throw new NotImplementedException(); }

        IEnumerable<IConfigurationSection> IConfiguration.GetChildren()
        {
            return _appConfiguration.GetChildren();
        }

        IChangeToken IConfiguration.GetReloadToken()
        {
            return _appConfiguration.GetReloadToken();
        }

        IConfigurationSection IConfiguration.GetSection(string key)
        {
            return _appConfiguration.GetSection(key);
        }
    }
}
