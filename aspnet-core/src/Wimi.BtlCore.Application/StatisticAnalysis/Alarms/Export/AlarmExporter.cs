using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Export
{
    public class AlarmExporter : EpPlusExcelExporterBase, IAlarmExporter
    {
        public FileDto ExportToFile(IEnumerable<AlarmExportDto> input)
        {
            var fileName = $"{this.L("AlarmStatistics")}_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("AlarmStatistics"));

                    this.AddHeader(
                        sheet,
                        this.L("StatisticalDate"),
                        this.L("MachineName"),
                        this.L("AlarmCode"),
                        this.L("NumberOfOccurrences"),
                        this.L("Proportion"),
                        this.L("DevicesAlarms")
                    );

                    this.AddObjects(sheet, 2 , input.ToList(),
                        s => s.SummaryDate,
                        s => s.MachineName,
                        s => s.Code,
                        s => s.Count,
                        s => s.RateName,
                        s => s.Message);

                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}