using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.ThirdpartyApis.Dto;
using WIMI.BTL.Machines.RepositoryDto.State;

namespace Wimi.BtlCore.BasicData.Machines.Repository
{
    public interface IStateRepository:ITransientDependency
    {
        Task<ShiftDefailInfo> GetShiftDefailInfo(int machineShiftDetailId);

        string GetMaxSyncStateDateTime();

        Task<IEnumerable<ListMahcineStateMapDto>> ListMahcineStateMaps(DateTime startTime, DateTime endTime, List<long> machineIdList, List<string> unionTables);

        Task<IEnumerable<ListMahcineStateMapDto>> ListMahcineStateMapsByShift(DateTime shiftDay, int machineId, List<string> unionTables);
        
        IEnumerable<dynamic> QueryCapacitiesBetweenStartTimeAndEndTime(DateTime startTime, DateTime endTime, List<string> unionTables);

        IEnumerable<dynamic> QueryStatesBetweenStartTimeAndEndTime(DateTime startTime, DateTime endTime, List<string> unionTables, bool byUser = false);

        Task<List<MachineStateRateDto>> GetMachineStateRateData(GetMachineStateRateInputDto input);

        Task<IEnumerable<MachineStateRateOutputDto>> GetMachineStateRate(int? machineId, IEnumerable<ShiftCalendarDto> correctedQueryDateList);

        Task<IEnumerable<GetOriginalState>> GetOriginalState(List<int> machineIdList, DateTime? startTime, DateTime? endTime);

        Task<IEnumerable<GetOriginalState>> GetOriginalReasonState(List<int> machineIdList, DateTime? startTime, DateTime? endTime);

        Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate(List<int> machineIdList, DateTime startTime, DateTime endTime, int? machineId);

        Task<IEnumerable<MachineYieldAnalysisOutputDto>> GetMachineData(List<int> machineIdList);

        Task<List<MachineYieldAnalysisOutputDto>> GetMachineYieldData(List<int> machineIdList, DateTime startTime, DateTime endTime);
    }
}