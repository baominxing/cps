using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Shift.Dtos;
using Wimi.BtlCore.Shifts.Dto;

namespace Wimi.BtlCore.Shifts
{
    public interface IShiftAppService : IApplicationService
    {
        Task<string> CheckBeforeDeleteMachineShiftSolution(MachineShiftSolutionInputDto input);

        Task<bool> CheckIfHasDevicesAssociatedShiftSolution(DeviceGroupShiftSolutionInputDto input);

        Task<bool> CreateDeviceGroupShiftSolution(DeviceGroupShiftSolutionInputDto input);

        Task CreateDeviceShiftSollution(DeviceShiftSolutionInputDto input);

        Task CreateShiftInfo(ShiftInfoInputDto input);

        Task CreateShiftSolution(ShiftSolutionInputDto input);

        Task DeleteMachineShiftSolution(MachineShiftSolutionInputDto input);

        void DeleteShiftSolution(EntityDto<int> input);

        Task DeleteShiftSolutionItem(EntityDto input);

        Task<ShiftSolutionDto> EditShiftSolution(ShiftSolutionInputDto input);

        Task<ListResultDto<ShiftSolutionDto>> GetDeviceGroups();

        Task<IEnumerable<DeviceHistoryShiftInfoDto>> GetDeviceHistoryShiftInfo(DeviceHistoryShiftInfoInputDto input);

        Task<Machine4ShiftSolutionDto> GetMachineShiftSolution(Machine4ShiftSolutionInputDto input);

        Task<IEnumerable<MachineShiftSolutionDto>> GetMachineShiftSolutions(MachineShiftSolutionInputDto input);

        Task<PagedResultDto<ShiftInfoDto>> GetShiftInfos(ShiftSolutionInputDto input);

        Task<List<ShiftInfoDto>> GetShiftInfosForModal(ShiftSolutionInputDto input);

        Task<ShiftSolutionDto> GetShiftSolutionForModal(ShiftSolutionInputDto input);

        Task<ListResultDto<ShiftSolutionDto>> GetShiftSolutions();

        IEnumerable GetShiftName(int deviceGroupId);

        Task UpdateMachineShiftSolution(Machine4ShiftSolutionInputDto input);

        Task UpdateShiftInfo(ShiftInfoInputDto input);

        Task<string> CreateMultiMachineShift(MultiMachineShiftInputDto input);

        Task BatchDeleteMachineShift(MultiMachineShiftInputDto input);
    }
}