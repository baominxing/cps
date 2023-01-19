using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Timing.Dto;

namespace Wimi.BtlCore.Timing
{
    public class TimingAppService : BtlCoreAppServiceBase, ITimingAppService
    {
        private readonly ITimeZoneService timeZoneService;

        public TimingAppService(ITimeZoneService timeZoneService)
        {
            this.timeZoneService = timeZoneService;
        }

        [HttpPost]
        public async Task<List<ComboboxItemDto>> GetEditionComboboxItems(GetTimezoneComboboxItemsInputDto input)
        {
            var timeZones = await this.GetTimezoneInfos(input.DefaultTimezoneScope);
            var timeZoneItems =
                new ListResultDto<ComboboxItemDto>(timeZones.Select(e => new ComboboxItemDto(e.Value, e.Name)).ToList())
                    .Items.ToList();

            if (!string.IsNullOrEmpty(input.SelectedTimezoneId))
            {
                var selectedEdition = timeZoneItems.FirstOrDefault(e => e.Value == input.SelectedTimezoneId);
                if (selectedEdition != null)
                {
                    selectedEdition.IsSelected = true;
                }
            }

            return timeZoneItems;
        }

        [HttpPost]
        public async Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInputDto input)
        {
            var timeZones = await this.GetTimezoneInfos(input.DefaultTimezoneScope);
            return new ListResultDto<NameValueDto>(timeZones);
        }

        private async Task<List<NameValueDto>> GetTimezoneInfos(SettingScopes defaultTimezoneScope)
        {
            var defaultTimezoneId =
                await this.timeZoneService.GetDefaultTimezoneAsync(defaultTimezoneScope, this.AbpSession.TenantId);
            var defaultTimezone = TimeZoneInfo.FindSystemTimeZoneById(defaultTimezoneId);
            var defaultTimezoneName = $"{this.L("Default")} [{defaultTimezone.DisplayName}]";

            var timeZones =
                TimeZoneInfo.GetSystemTimeZones().Select(tz => new NameValueDto(tz.DisplayName, tz.Id)).ToList();

            timeZones.Insert(0, new NameValueDto(defaultTimezoneName, string.Empty));
            return timeZones;
        }
    }
}