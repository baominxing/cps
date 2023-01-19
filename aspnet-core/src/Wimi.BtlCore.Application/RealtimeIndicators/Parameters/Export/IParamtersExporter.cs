using Wimi.BtlCore.Dto;
using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;

namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Export
{
    public interface IParamtersExporter
    {
        FileDto ExportToFile(GetHistoryParamtersListExportDto input);
    }
}
