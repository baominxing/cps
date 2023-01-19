using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Zero.Configuration;
using Microsoft.Extensions.Configuration;

namespace Wimi.BtlCore.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AppSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            //return new[]
            //{
            //    new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true)
            //};

            return GetHostSettings().Union(GetTenantSettings());
        }

        private IEnumerable<SettingDefinition> GetHostSettings()
        {

            return new[] {
                 new SettingDefinition(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,"true"),
                new SettingDefinition(AppSettings.General.WebSiteRootAddress, "http://localhost"),
                new SettingDefinition(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com"),
                new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer"),
                new SettingDefinition(EmailSettingNames.Smtp.Host,"127.0.0.1"),
                new SettingDefinition(EmailSettingNames.Smtp.UserName, string.Empty),
                new SettingDefinition(EmailSettingNames.Smtp.Password, string.Empty),
                new SettingDefinition(EmailSettingNames.Smtp.Domain, string.Empty),
                new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "false"),
                new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials,"true"),
                new SettingDefinition(VersionSettingNames.Ico,    System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.Ico] ?? "favicon.ico", isVisibleToClients: true),
                new SettingDefinition(VersionSettingNames.AppName, System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.AppName] ?? AppVersionHelper.AppName, isVisibleToClients: true),
                new SettingDefinition(VersionSettingNames.Copyright, System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.Copyright] ?? "Copyright",  isVisibleToClients: true),
                new SettingDefinition(VersionSettingNames.PageSizeOptions, System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.PageSizeOptions] ?? AppSettings.Page.PageSizeOptions,  isVisibleToClients: true),
                new SettingDefinition(AppSettings.CutterManagement.LifeMethod,"bycomponent"),
                new SettingDefinition(AppSettings.TenantManagement.AllowSelfRegistration, GetFromAppSettings(AppSettings.TenantManagement.AllowSelfRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault, GetFromAppSettings(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault, "false")),
                new SettingDefinition(AppSettings.TenantManagement.UseCaptchaOnRegistration, GetFromAppSettings(AppSettings.TenantManagement.UseCaptchaOnRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettings.TenantManagement.DefaultEdition, GetFromAppSettings(AppSettings.TenantManagement.DefaultEdition, "")),
                
                //new SettingDefinition(AppSettings.Database.ConnectionString,GetFromSettings(AppSettings.Database.ConnectionString)),
                //new SettingDefinition(AppSettings.General.WebSiteRootAddress,GetFromAppSettings(AppSettings.General.WebSiteRootAddress)),

                

                //new SettingDefinition(AppSettings.MongodbDatabase.ConnectionString,GetFromAppSettings(AppSettings.MongodbDatabase.ConnectionString)),
                //new SettingDefinition(AppSettings.MongodbDatabase.DatabaseName,GetFromAppSettings(AppSettings.MongodbDatabase.DatabaseName)),
                //new SettingDefinition(AppSettings.MongodbDatabase.SyncMongoDataTimerPeriod,GetFromAppSettings(AppSettings.MongodbDatabase.SyncMongoDataTimerPeriod,"5")),
                //new SettingDefinition(AppSettings.Shift.ShiftTimeDuration,GetFromAppSettings(AppSettings.Shift.ShiftTimeDuration,"30")),
                //new SettingDefinition(AppSettings.Shift.ShiftTimeOutside,GetFromAppSettings(AppSettings.Shift.ShiftTimeOutside,"true")),
                //new SettingDefinition(AppSettings.CutterManagement.LifeMethod,GetFromAppSettings(AppSettings.CutterManagement.LifeMethod,"0")),

                

                //new SettingDefinition(AppSettings.TraceabilityConfig.OfflineYield,GetFromAppSettings(AppSettings.TraceabilityConfig.OfflineYield,"true")),
                //new SettingDefinition(AppSettings.BackgroudJobConfig.RefillAlarmFeatureEnabled,GetFromAppSettings(AppSettings.BackgroudJobConfig.RefillAlarmFeatureEnabled,"false")),

                new SettingDefinition(AppSettings.MachineParameter.FixedDataItems,GetFromAppSettings(AppSettings.MachineParameter.FixedDataItems)),
                new SettingDefinition(AppSettings.MachineParameter.GaugeParameters,GetFromAppSettings(AppSettings.MachineParameter.GaugeParameters)),
                new SettingDefinition(AppSettings.FooterControl.IsShowCopyRight,GetFromAppSettings(AppSettings.FooterControl.IsShowCopyRight, "true")),
                new SettingDefinition(AppSettings.Visual.StartHourInGantChart,GetFromAppSettings(AppSettings.Visual.StartHourInGantChart,"10"))
                
            };
                }

        private IEnumerable<SettingDefinition> GetTenantSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettings.UserManagement.AllowSelfRegistration, GetFromAppSettings(AppSettings.UserManagement.AllowSelfRegistration, "true"), scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, GetFromAppSettings(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault, "false"), scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.UserManagement.UseCaptchaOnRegistration, GetFromAppSettings(AppSettings.UserManagement.UseCaptchaOnRegistration, "true"), scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.UseCaptchaOnRegistration,System.Configuration.ConfigurationManager.AppSettings[AppSettings.UserManagement.UseCaptchaOnRegistration]??"true",scopes:SettingScopes.Tenant),
                new SettingDefinition(VersionSettingNames.AppName,System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.AppName]??AppVersionHelper.AppName,isVisibleToClients:true),
                new SettingDefinition(VersionSettingNames.Ico,System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.Ico]??"favicon.ico",isVisibleToClients:true),
                new SettingDefinition(VersionSettingNames.Copyright,System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.Copyright]??"Copyright",isVisibleToClients:true),
                new SettingDefinition(VersionSettingNames.PageSizeOptions,System.Configuration.ConfigurationManager.AppSettings[VersionSettingNames.PageSizeOptions]??AppSettings.Page.PageSizeOptions,isVisibleToClients:true),
            };
        }

        private string GetFromAppSettings(string name, string defaultValue = null)
        {
            return GetFromSettings("App:" + name, defaultValue);
        }

        private string GetFromSettings(string name, string defaultValue = null)
        {
            return _appConfiguration[name] ?? defaultValue;
        }
    }
}
