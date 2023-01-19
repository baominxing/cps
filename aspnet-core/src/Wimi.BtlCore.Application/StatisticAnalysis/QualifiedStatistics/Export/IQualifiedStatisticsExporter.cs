using System.Collections.Generic;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Export
{
    public interface IQualifiedStatisticsExporter
    {
        FileDto ExportToFile(IEnumerable<QualifiedStatisticsDto> input);
    }
}
