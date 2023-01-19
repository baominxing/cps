using Abp.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Configuration.Host.Dto;
using Wimi.BtlCore.Configuration.Tenants.Dto;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.Timing;

namespace Wimi.BtlCore.Configuration.Tenants
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public class TenantSettingsAppService : BtlCoreAppServiceBase, ITenantSettingsAppService
    {
        private readonly IMultiTenancyConfig multiTenancyConfig;

        private readonly ITimeZoneService timeZoneService;

        public TenantSettingsAppService(
            IMultiTenancyConfig multiTenancyConfig,
            ITimeZoneService timeZoneService)
        {
            this.multiTenancyConfig = multiTenancyConfig;
            this.timeZoneService = timeZoneService;
        }

        [HttpPost]
        public async Task<TenantSettingsEditDto> GetAllSettings()
        {
            var settings = new TenantSettingsEditDto
            {
                UserManagement = new TenantUserManagementSettingsEditDto
                {
                    AllowSelfRegistration = await this.SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowSelfRegistration),
                    IsNewRegisteredUserActiveByDefault = await this.SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault),
                    IsEmailConfirmationRequiredForLogin = await this.SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin),
                    UseCaptchaOnRegistration = await this.SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration)
                }
            };

            if (!this.multiTenancyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                // General
                settings.General = new GeneralSettingsEditDto();
                if (!this.multiTenancyConfig.IsEnabled)
                {
                    settings.General.WebSiteRootAddress = await this.SettingManager.GetSettingValueAsync(AppSettings.General.WebSiteRootAddress);
                }

                if (Clock.SupportsMultipleTimezone)
                {
                    var timezone = await this.SettingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, this.AbpSession.GetTenantId());

                    settings.General.Timezone = timezone;
                    settings.General.TimezoneForComparison = timezone;
                }

                var defaultTimeZoneId = await this.timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, this.AbpSession.TenantId);

                if (settings.General.Timezone == defaultTimeZoneId)
                {
                    settings.General.Timezone = string.Empty;
                }
            }

            if (!this.multiTenancyConfig.IsEnabled)
            {
                // Email
                settings.Email = new EmailSettingsEditDto
                {
                    DefaultFromAddress = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress),
                    DefaultFromDisplayName = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromDisplayName),
                    SmtpHost = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host),
                    SmtpPort = await this.SettingManager.GetSettingValueAsync<int>(EmailSettingNames.Smtp.Port),
                    SmtpUserName = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName),
                    SmtpPassword = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password),
                    SmtpDomain = await this.SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Domain),
                    SmtpEnableSsl = await this.SettingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                    SmtpUseDefaultCredentials = await this.SettingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)
                };
            }

            settings.Version = new VersionSettingsEditDto()
            {
                Ico = await this.SettingManager.GetSettingValueForApplicationAsync(VersionSettingNames.Ico),
                AppName = await this.SettingManager.GetSettingValueForApplicationAsync(VersionSettingNames.AppName),
                CopyRight = await this.SettingManager.GetSettingValueForApplicationAsync(VersionSettingNames.Copyright),
                PageSizeOptions = await this.SettingManager.GetSettingValueForApplicationAsync(VersionSettingNames.PageSizeOptions),

            };

            settings.Cutter = new CutterSettingEditDto()
            {
                CutterLifeMethod = (await this.SettingManager.GetSettingValueForApplicationAsync(AppSettings.CutterManagement.LifeMethod)) == "bycomponent" ? EnumCutterLifeMethod.ByComponent : EnumCutterLifeMethod.ByCount
            };

            return settings;
        }

        public async Task UpdateAllSettings(TenantSettingsEditDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Version.PageSizeOptions))
            {
                var PageSizeOptionStr = input.Version.PageSizeOptions.Split(",");
                foreach (var item in PageSizeOptionStr)
                {
                    if (!int.TryParse(item, out var n))
                    {
                        throw new UserFriendlyException(this.L("PageSizeOptions{0}WrongFormat", input.Version.PageSizeOptions));
                    }
                }
            }

            // User management
            await
                this.SettingManager.ChangeSettingForTenantAsync(
                    this.AbpSession.GetTenantId(),
                    AppSettings.UserManagement.AllowSelfRegistration,
                    input.UserManagement.AllowSelfRegistration.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
            await
                this.SettingManager.ChangeSettingForTenantAsync(
                    this.AbpSession.GetTenantId(),
                    AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                    input.UserManagement.IsNewRegisteredUserActiveByDefault.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
            await
                this.SettingManager.ChangeSettingForTenantAsync(
                    this.AbpSession.GetTenantId(),
                    AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                    input.UserManagement.IsEmailConfirmationRequiredForLogin.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
            await
                this.SettingManager.ChangeSettingForTenantAsync(
                    this.AbpSession.GetTenantId(),
                    AppSettings.UserManagement.UseCaptchaOnRegistration,
                    input.UserManagement.UseCaptchaOnRegistration.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));

            await
               this.SettingManager.ChangeSettingForApplicationAsync(
                   AppSettings.CutterManagement.LifeMethod,
                   input.Cutter.CutterLifeMethod.ToString().ToLower(CultureInfo.InvariantCulture));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.General.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await this.timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, this.AbpSession.TenantId);
                    await this.SettingManager.ChangeSettingForTenantAsync(this.AbpSession.GetTenantId(), TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await this.SettingManager.ChangeSettingForTenantAsync(this.AbpSession.GetTenantId(), TimingSettingNames.TimeZone, input.General.Timezone);
                }
            }

            if (!this.multiTenancyConfig.IsEnabled)
            {
                input.ValidateHostSettings();

                // General
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        AppSettings.General.WebSiteRootAddress,
                        input.General.WebSiteRootAddress.EnsureEndsWith('/'));

                // Email
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.DefaultFromAddress,
                        input.Email.DefaultFromAddress);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.DefaultFromDisplayName,
                        input.Email.DefaultFromDisplayName);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.Host,
                        input.Email.SmtpHost);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.Port,
                        input.Email.SmtpPort.ToString(CultureInfo.InvariantCulture));
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.UserName,
                        input.Email.SmtpUserName);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.Password,
                        input.Email.SmtpPassword);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.Domain,
                        input.Email.SmtpDomain);
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.EnableSsl,
                        input.Email.SmtpEnableSsl.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        EmailSettingNames.Smtp.UseDefaultCredentials,
                        input.Email.SmtpUseDefaultCredentials.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));

                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        VersionSettingNames.Ico,
                        input.Version.Ico.ToString(CultureInfo.InvariantCulture));

                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        VersionSettingNames.AppName,
                        input.Version.AppName.ToString(CultureInfo.InvariantCulture));

                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        VersionSettingNames.PageSizeOptions,
                        string.IsNullOrWhiteSpace(input.Version.PageSizeOptions) ? AppSettings.Page.PageSizeOptions : input.Version.PageSizeOptions);

                await
                    this.SettingManager.ChangeSettingForApplicationAsync(
                        VersionSettingNames.Copyright,
                        input.Version.CopyRight.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}