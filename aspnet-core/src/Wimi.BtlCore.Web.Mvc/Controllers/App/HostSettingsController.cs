using Abp.AspNetCore.Mvc.Authorization;
using Abp.Configuration;
using Abp.Runtime.Session;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Configuration.Host;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Editions;
using Wimi.BtlCore.Timing;
using Wimi.BtlCore.Timing.Dto;
using Wimi.BtlCore.Web.Models.App;

namespace Wimi.BtlCore.Web.Controllers.App
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Host_Settings)]
    public class HostSettingsController : BtlCoreControllerBase
    {
        private readonly IEditionAppService editionAppService;

        private readonly IHostSettingsAppService hostSettingsAppService;

        private readonly ITimingAppService timingAppService;

        private readonly UserManager userManager;

        public HostSettingsController(
            IHostSettingsAppService hostSettingsAppService,
            UserManager userManager,
            IEditionAppService editionAppService,
            ITimingAppService timingAppService)
        {
            this.hostSettingsAppService = hostSettingsAppService;
            this.userManager = userManager;
            this.editionAppService = editionAppService;
            this.timingAppService = timingAppService;
        }

        public async Task<ActionResult> Index()
        {
            var hostSettings = await this.hostSettingsAppService.GetAllSettings();
            var editionItems =
                await this.editionAppService.GetEditionComboboxItems(hostSettings.TenantManagement.DefaultEditionId);
            var timezoneItems =
                await
                this.timingAppService.GetEditionComboboxItems(
                    new GetTimezoneComboboxItemsInputDto
                    {
                        DefaultTimezoneScope = SettingScopes.Application,
                        SelectedTimezoneId =
                                await
                                this.SettingManager.GetSettingValueForApplicationAsync(
                                    TimingSettingNames.TimeZone)
                    });

            var currentuser = await this.userManager.GetUserByIdAsync(this.AbpSession.GetUserId());

            this.ViewBag.CurrentUserEmail = await this.userManager.GetEmailAsync(currentuser);

            var model = new HostSettingsViewModel
            {
                Settings = hostSettings,
                EditionItems = editionItems,
                TimezoneItems = timezoneItems
            };

            return this.View(model);
        }
    }
}
