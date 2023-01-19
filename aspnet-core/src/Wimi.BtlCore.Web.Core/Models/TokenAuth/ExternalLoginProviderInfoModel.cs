using Abp.AutoMapper;
using Wimi.BtlCore.Authentication.External;

namespace Wimi.BtlCore.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
