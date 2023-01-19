using Abp.Configuration;

namespace Wimi.BtlCore.Timing.Dto
{
    public class GetTimezoneComboboxItemsInputDto
    {
        public SettingScopes DefaultTimezoneScope { get; set; }
        
        public string SelectedTimezoneId { get; set; }
    }
}