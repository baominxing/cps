using Abp.AspNetCore.Mvc.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Configuration.Tenants;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Timing;
using Wimi.BtlCore.Timing.Dto;
using Wimi.BtlCore.Web.Models.Settings;

namespace Wimi.BtlCore.Web.Controllers.Settings
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public class SettingsController : BtlCoreControllerBase
    {
        private readonly IMultiTenancyConfig multiTenancyConfig;

        private readonly ITenantSettingsAppService tenantSettingsAppService;

        private readonly ITimingAppService timingAppService;

        public SettingsController(
            ITenantSettingsAppService tenantSettingsAppService, 
            IMultiTenancyConfig multiTenancyConfig, 
            ITimingAppService timingAppService)
        {
            this.tenantSettingsAppService = tenantSettingsAppService;
            this.multiTenancyConfig = multiTenancyConfig;
            this.timingAppService = timingAppService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await this.tenantSettingsAppService.GetAllSettings();
            this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;

            var timezoneItems =
                await
                this.timingAppService.GetEditionComboboxItems(
                    new GetTimezoneComboboxItemsInputDto
                        {
                            DefaultTimezoneScope = SettingScopes.Tenant, 
                            SelectedTimezoneId =
                                await
                                this.SettingManager.GetSettingValueForTenantAsync(
                                    TimingSettingNames.TimeZone, 
                                    this.AbpSession.GetTenantId())
                        });

            var model = new SettingsViewModel { Settings = output, TimezoneItems = timezoneItems };

            return this.View("Views/App/Settings/Index.cshtml", model);
        }

        public PartialViewResult LogModal()
        {
            return this.PartialView("Views/App/Settings/_LogModal.cshtml");
        }
    }
}