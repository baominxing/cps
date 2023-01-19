using Abp.AutoMapper;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.MultiTenancy.Dashboard.Dto
{
    [AutoMap(typeof(MachineActivationApiDto))]
    public class MachineActivationDto: MachineActivationApiDto
    {
       
    }
}