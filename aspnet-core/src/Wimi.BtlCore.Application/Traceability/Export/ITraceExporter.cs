using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Traceability.Dto;

namespace Wimi.BtlCore.Traceability.Export
{
    public interface ITraceExporter
    {
        FileDto ExportToFile(List<TraceExportItemDto> input);

        FileDto ExportToFile(List<NGPartsExportDto> input, System.DateTime? starTime, System.DateTime? endTime, NgPartsRequestDto searchParam);
    }
}
