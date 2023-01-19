using System.Collections.Generic;

using Abp.Application.Services.Dto;
using Wimi.BtlCore.Configuration.Host.Dto;

namespace Wimi.BtlCore.Web.Models.App
{
    public class HostSettingsViewModel
    {
        public List<ComboboxItemDto> EditionItems { get; set; }

        public HostSettingsEditDto Settings { get; set; }

        public List<ComboboxItemDto> TimezoneItems { get; set; }
    }
}
