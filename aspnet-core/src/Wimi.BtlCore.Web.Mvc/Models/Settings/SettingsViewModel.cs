using Abp.Application.Services.Dto;
using System.Collections.Generic;
using Wimi.BtlCore.Configuration.Tenants.Dto;

namespace Wimi.BtlCore.Web.Models.Settings
{
    public class SettingsViewModel
    {
        public TenantSettingsEditDto Settings { get; set; }

        public List<ComboboxItemDto> TimezoneItems { get; set; }
    }
}