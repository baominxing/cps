using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Export
{
    public class QualifiedStatisticsExporter : EpPlusExcelExporterBase, IQualifiedStatisticsExporter
    {
        public FileDto ExportToFile(IEnumerable<QualifiedStatisticsDto> input)
        {
            var fileName = $"{this.L("PassRate")}_{DateTime.Now.ToMongoDateTime()}.xlsx";
            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("PassRateReport"));

                    for (var i = 0; i < input.Count(); i++)
                    {
                        var item = input.ToArray()[i];

                        this.AddVerticalHeader(
                            sheet,
                            1 + i * 7,
                            item.DisplayName,
                            this.L("OnlineCount"),
                            this.L("QualifiedOfflineNumber"),
                            this.L("NgCount"),
                            this.L("NumberOfProcessing"),
                            this.L("PassRate"),
                            " "
                        );

                        this.AddVerticalObjects(sheet, 1 + i * 7, item.Items.ToList(), 2,
                            s => s.SummaryDate,
                            s => s.OnlineCount,
                            s => s.QualifiedOfflineCount,
                            s => s.NgCount,
                            s => s.ProcessingCount,
                            s => s.QualifiedTableRate);

                    }
                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}
