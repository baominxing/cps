using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;

namespace Wimi.BtlCore.BasicData.Machines.Repository
{
    public interface IAlarmRepository : ITransientDependency
    {
        Task<IEnumerable<MachineAlarmStatistices>> GetAlarmChartData(GetMachineAlarms input);

        Task<int> GetAlarmChartDataCount(GetMachineAlarms input);

        Task<IEnumerable<MachineAlarmStatistices>> GetAlarmDetailData(GetMachineAlarms input);

        Task<IEnumerable<MachineAlarmStatistices>> GetAlarmDetailDataForModal(GetMachineAlarms input);

        Task<IEnumerable<MachineAlarmStatistices>> GetQueriedMachineInfo(GetMachineAlarms input);

        string GetMaxSyncAlarmDateTime();

        Task ImportDataByCover(ImportDataDto input);

        Task ImportDataByIncrement(ImportDataDto input);

        Task<IEnumerable<AlarmExportDto>> QueryAlarmExportData(string statisticalWayStr, DateTime? startTime, DateTime? endTime, List<int> machineIdList, List<int> machineShiftSolutionIdList);

        List<string> GetUnionTables(DateTime startTime, DateTime endTime);
    }
}