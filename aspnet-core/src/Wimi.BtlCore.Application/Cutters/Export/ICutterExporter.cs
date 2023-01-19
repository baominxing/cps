using System;
using System.Collections.Generic;
using System.Text;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Cutters.Export
{
    public interface ICutterExporter
    {
        FileDto ExportToFile(List<CutterLoadAndUnloadRecordDto> input, System.DateTime? starTime, System.DateTime? endTime, QueryCutterRecordDto searchParam);

        FileDto ExportToFile(List<CutterStatesDto> input, QueryCutterStateDto searchParam);
    }
}
