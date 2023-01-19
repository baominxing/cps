using Abp.Configuration;

namespace Wimi.BtlCore.Timing.Dto
{
    public class GetTimezonesInputDto
    {
        public SettingScopes DefaultTimezoneScope { get; set; }
    }
}