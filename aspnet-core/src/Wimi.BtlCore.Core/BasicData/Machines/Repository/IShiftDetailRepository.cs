using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.BasicData.Machines.Repository
{
    public interface IShiftDetailRepository : ITransientDependency
    {
        Task InsertMachineShiftDetailsAsync(DeviceShiftSolutionInput input);

        Task InsertMachineShiftCalendarAsync(DeviceShiftSolutionInput input);

        MachineShiftDetail GetCurrentShiftDay();

        IEnumerable<ShiftCalendarDto> CorrectQueryDate(GetMachineStateRateInputDto input);

        void DeleteShiftDetailsAndCalender(int machineId, int shiftSolutonId, DateTime? shiftDayGt = null, DateTime? shiftDayLt = null, EnumEqType eqType = EnumEqType.None);

        IEnumerable<ShiftCalendarDto> GetShiftCalendarsByShiftIds(List<int> shiftIds);

        Task DeleteShiftOldData(MachineShiftEffectiveInterval input);

        List<string> CheckIfCrossedWithLastedEffectiveShiftSolution(string startTime, string endTime);
    }
}
