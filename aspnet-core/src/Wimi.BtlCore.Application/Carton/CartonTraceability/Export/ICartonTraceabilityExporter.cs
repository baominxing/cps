using System;
using System.Collections.Generic;
using System.Text;
using Wimi.BtlCore.Carton.CartonTraceability.Dtos;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Carton.CartonTraceability.Export
{
    public interface ICartonTraceabilityExporter
    {
        FileDto ExportCartonToFile(List<CartonExportDto> input);
    }
}
