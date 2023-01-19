using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.Common.Dtos;

namespace Wimi.BtlCore.Common
{
    public interface ICommonRepository : ITransientDependency
    {
        List<AlarmInfo> RefillAlarmMessage();

        string CallSpToSyncEndTime(bool alarmResult, bool stateResult, bool capacityResult);

        void UpdateColumnsAfterSyncState();

        Task WriteLanguageToDatabase(MultiLanguageInputDto languageData);

        List<string> GetUnionTables(DateTime startTime, DateTime endTime, string tableName);

        List<DateTime> GetSummaryDateList(DateTime startTime, DateTime endTime);

        Task<List<ShiftEffSummary>> GetShiftEffSummyList(string workShopCode);

        Task<List<UtilizationRate>> GetShiftUtilizationRateList(string workShopCode);

    }
}
