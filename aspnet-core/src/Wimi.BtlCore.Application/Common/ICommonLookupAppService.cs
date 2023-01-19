namespace Wimi.BtlCore.Common
{
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.Common.Dto;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Order.Processes.Dtos;

    public interface ICommonLookupAppService : IApplicationService
    {
        Task<PagedResultDto<NameValueDto>> FindMachines(FindMachinesInputDto input);

        Task<PagedResultDto<NameValueDto>> FindMachinesToDmp(FindMachinesInputDto input);

        Task<PagedResultDto<ProcessDto>> FindProcesses(FindProcessInputDto input);

        Task<DatatablesPagedResultOutput<NameValueDto>> FindUsers(FindUsersInputDto input);

        Task<DeviceGroupAndMachineWithPermissionsDto> GetDeviceGroupAndMachineWithPermissions();

        Task<DeviceGroupWithPermissionsDto> GetDeviceGroupWithPermissions();

        Task<ListResultDto<ComboboxItemDto>> GetEditionsForCombobox();

        Task<ListResultDto<MachineSettingListDto>> GetMachines(FindMachinesInputDto input);

        Task<ListResultDto<MachineSettingListDto>> GetMachinesForTenantSpecific(FindMachinesInputDto input);

        Task<PagedResultDto<NameValueDto<int>>> FindFmsCutters(FindFmsCuttersInput input);

        Task<DeviceGroupAndMachineWithPermissionsDto> GetDeviceGroupAndDefaultCountMachineWithPermissions();

        int DefaultSelectedMachineCount();
    }
}