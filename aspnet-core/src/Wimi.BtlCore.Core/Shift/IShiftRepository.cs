using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Shift.Dtos;

namespace Wimi.BtlCore.Shift
{
    public interface IShiftRepository: ITransientDependency
    {
        Task<IEnumerable<DeviceHistoryShiftInfoDto>> GetDeviceHistoryShiftInfo(int deviceId);

        Task<IEnumerable<MachineShiftSolutionDto>> GetMachineShiftSolutions(string ids, int? shiftSolutionId,int start, int length, string queryType);

        Task<bool> CheckIfCurrentDayShiftIsWorking(int machineId, int? id);

        Task BatchDeleteMachineShift(List<int> machineIdList);
    }
}
