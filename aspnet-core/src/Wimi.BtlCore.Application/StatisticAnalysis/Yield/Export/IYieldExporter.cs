using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Export
{
    public interface IYieldExporter
    {
        FileDto ExportToFile(MachineYieldDto input);
    }
}