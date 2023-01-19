using System;
using System.Globalization;
using System.Threading.Tasks;

using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Timing;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Configuration.Host.Dto;
using Wimi.BtlCore.Editions;
using Wimi.BtlCore.Timing;

namespace Wimi.BtlCore.Configuration.Host
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Host_Settings)]
    public class HostSettingsAppService : BtlCoreAppServiceBase, IHostSettingsAppService
    {
        private readonly EditionManager editionManager;

        private readonly IEmailSender emailSender;

        private readonly ITimeZoneService timeZoneService;

        public HostSettingsAppService(
            IEmailSender emailSender, 
            EditionManager editionManager, 
            ITimeZoneService timeZoneService)
        {
            this.emailSender = emailSender;
            this.editionManager = editionManager;
            this.timeZoneService = timeZoneService;
        }

        [HttpPost]
        public async Task<HostSettingsEditDto> GetAllSettings()
        {
            var timezone = await this.SettingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            var hostSettings = new HostSettingsEditDto
                                   {
                                       General =
                                           new GeneralSettingsEditDto
                                               {
                                                   WebSiteRootAddress =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                          AppSettings.General.WebSiteRootAddress), 
                                                   Timezone = timezone, 
                                                   TimezoneForComparison =
                                                       timezone
                                               }, 
                                       TenantManagement =
                                           new TenantManagementSettingsEditDto
                                               {
                                                   AllowSelfRegistration
                                                       =
                                                       await
                                                       this
                                                           .SettingManager
                                                           .GetSettingValueAsync<bool>(AppSettings.TenantManagement
                                                           .AllowSelfRegistration), 
                                                   IsNewRegisteredTenantActiveByDefault
                                                       =
                                                       await
                                                       this
                                                           .SettingManager
                                                           .GetSettingValueAsync<bool>(AppSettings
                                                           .TenantManagement
                                                           .IsNewRegisteredTenantActiveByDefault), 
                                                   UseCaptchaOnRegistration
                                                       =
                                                       await
                                                       this
                                                           .SettingManager
                                                           .GetSettingValueAsync<bool>(
                                                               AppSettings
                                                           .TenantManagement
                                                           .UseCaptchaOnRegistration)
                                               }, 
                                       UserManagement =
                                           new HostUserManagementSettingsEditDto
                                               {
                                                   IsEmailConfirmationRequiredForLogin
                                                       =
                                                       await
                                                       this
                                                           .SettingManager
                                                           .GetSettingValueAsync<bool>(
                                                               AbpZeroSettingNames
                                                           .UserManagement
                                                           .IsEmailConfirmationRequiredForLogin)
                                               }, 
                                       Email =
                                           new EmailSettingsEditDto
                                               {
                                                   DefaultFromAddress =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .DefaultFromAddress), 
                                                   DefaultFromDisplayName =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .DefaultFromDisplayName), 
                                                   SmtpHost =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .Smtp.Host), 
                                                   SmtpPort =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync<int>(
                                                               EmailSettingNames
                                                           .Smtp.Port), 
                                                   SmtpUserName =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .Smtp.UserName), 
                                                   SmtpPassword =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .Smtp.Password), 
                                                   SmtpDomain =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync(
                                                               EmailSettingNames
                                                           .Smtp.Domain), 
                                                   SmtpEnableSsl =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync<bool>(
                                                               EmailSettingNames
                                                           .Smtp.EnableSsl), 
                                                   SmtpUseDefaultCredentials =
                                                       await
                                                       this.SettingManager
                                                           .GetSettingValueAsync<bool>(
                                                               EmailSettingNames
                                                           .Smtp
                                                           .UseDefaultCredentials)
                                               }
                                   };

            var defaultTenantId =
                await this.SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.DefaultEdition);
            if (!string.IsNullOrEmpty(defaultTenantId)
                && (await this.editionManager.FindByIdAsync(Convert.ToInt32(defaultTenantId)) != null))
            {
                hostSettings.TenantManagement.DefaultEditionId = Convert.ToInt32(defaultTenantId);
            }

            var defaultTimeZoneId =
                await this.timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, this.AbpSession.TenantId);
            if (hostSettings.General.Timezone == defaultTimeZoneId)
            {
                hostSettings.General.Timezone = string.Empty;
            }

            return hostSettings;
        }

        public async Task SendTestEmail(SendTestEmailInputDto input)
        {
            var subject = this.L("TestEmail_Subject");
            var body = this.L("TestEmail_Body");

            await this.emailSender.SendAsync(input.EmailAddress, subject, body);
        }

        public async Task UpdateAllSettings(HostSettingsEditDto input)
        {
            // General
            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                   AppSettings.General.WebSiteRootAddress, 
                    input.General.WebSiteRootAddress.EnsureEndsWith('/'));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.General.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await
                        this.timeZoneService.GetDefaultTimezoneAsync(
                            SettingScopes.Application, 
                            this.AbpSession.TenantId);
                    await
                        this.SettingManager.ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await
                        this.SettingManager.ChangeSettingForApplicationAsync(
                            TimingSettingNames.TimeZone, 
                            input.General.Timezone);
                }
            }

            // Tenant management
            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                    AppSettings.TenantManagement.AllowSelfRegistration, input.TenantManagement.AllowSelfRegistration.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                    AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault, 
                    input.TenantManagement.IsNewRegisteredTenantActiveByDefault.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));
            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                    AppSettings.TenantManagement.UseCaptchaOnRegistration, 
                    input.TenantManagement.UseCaptchaOnRegistration.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));

            var defaultEditionId = input.TenantManagement.DefaultEditionId?.ToString() ?? string.Empty;

            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                    AppSettings.TenantManagement.DefaultEdition, 
                    defaultEditionId);

            // User management
            await
                this.SettingManager.ChangeSettingForApplicationAsync(
                    AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, 
                    input.UserManagement.IsEmailConfirmationRequiredForLogin.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture));

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
                this.SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.Email.SmtpHost);
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
        }
    }
}