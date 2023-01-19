namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Wimi.BtlCore.DeviceGroups.Dto;

    public interface IDeviceGroupAppService : IApplicationService
    {
        Task<string> AddMachineListToDeviceGroup(MachineListToDeviceGroupInputDto input);

        Task AddMachineToDeviceGroup(MachineToDeviceGroupInputDto input);

        Task<DeviceGroupDto> CreateDeviceGroup(CreateDeviceGroupInputDto input);

        Task DeleteDeviceGroup(EntityDto input);

        Task<PagedResultDto<DeviceGroupMachineListDto>> GetDeviceGroupMachines(GetDeviceGroupMachinesInputDto input);

        Task<ListResultDto<DeviceGroupDto>> GetDeviceGroups();

        Task<IEnumerable<DeviceGroupDto>> ListFirstClassDeviceGroups();

        Task<bool> IsInDeviceGroup(MachineToDeviceGroupInputDto input);

        Task<DeviceGroupDto> MoveDeviceGroup(MoveDeviceGroupInputDto input);

        Task RemoveMachineFromDeviceGroup(MachineToDeviceGroupInputDto input);

        Task<DeviceGroupDto> UpdateDeviceGroup(UpdateDeviceGroupInputDto input);

        List<NameValueDto> ListRootDevices();
    }
}