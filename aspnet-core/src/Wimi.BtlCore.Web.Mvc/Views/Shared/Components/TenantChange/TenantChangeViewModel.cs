using Abp.AutoMapper;
using Wimi.BtlCore.AppSystem.Sessions.Dto;

namespace Wimi.BtlCore.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}
