using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.ShiftDayTimeRange
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Dependency;

    public interface IShiftDayTimeRangeRepository: ITransientDependency
    {
        Task<IEnumerable<ShiftDayTimeRange>> ListShiftDayTimeRanges(IEnumerable<int> machineIds, DateTime startTime, DateTime endTime);

        Task<IEnumerable<NameValueDto<int>>> ListSummaryDataRange(int startDateKey, int endDateKey);

        Task<IEnumerable<ShiftDayTimeRange>> ListMachineShiftDayTimeRange(int machineId, DateTime shiftDay);

        IEnumerable<ShiftDayTimeRange> ListMachineShiftDayTimeRange();

        string GetShiftItemName(int machineShiftDetailId);

        IEnumerable<ShiftEffectiveIntervalTimeRange> ListMachineShiftEffectiveIntervalTimeRange(int shiftSolutionId);

        MachineCapacityShiftDetail GetMachineCurrentShiftDetail(long shiftDetailId);

        IEnumerable<ShiftDayTimeRange> ListMachineShiftDayTimeRange(IEnumerable<int> machineIds);

        ShiftDayTimeRange ListMachineShiftDayTimeRange(int machineShiftDetailId);


        ShiftDayTimeRange GetMachineNaturalDayShift(DateTime day, int machineId);
    }
}