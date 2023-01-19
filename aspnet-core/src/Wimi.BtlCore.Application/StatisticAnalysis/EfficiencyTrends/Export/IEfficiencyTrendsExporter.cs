

using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Export
{
    public interface IEfficiencyTrendsExporter
    {
        FileDto ExportToFile(GetEfficiencyTrendsListDto input);
    }
}
