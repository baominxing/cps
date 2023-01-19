using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.States.Export
{
    public interface IStatesExporter
    {
        FileDto ExportToFile(IEnumerable<MachineStateRateDto> input);
    }
}
