using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Export
{
    public interface IAlarmExporter
    {
        FileDto ExportToFile(IEnumerable<AlarmExportDto> input);
    }
}