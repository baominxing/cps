using Abp.AutoMapper;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.MultiTenancy.Dashboard.Dto
{
    [AutoMap(typeof(CurrentMachineShiftInfoDto))]
    public class GetCurrentMachineShiftInfoDto:CurrentMachineShiftInfoDto
    {
      
    }
}