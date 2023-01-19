using System;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Export
{
    public class YieldExporter: EpPlusExcelExporterBase, IYieldExporter
    {
        public FileDto ExportToFile(MachineYieldDto input)
        {
            var fileName = $"{this.L("YieldStatistics")}_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("YieldStatistics"));

                    this.AddHeader(
                        sheet,
                        this.L("StatisticalDate"),
                        this.L("MachineName"),
                        this.L("Yield")
                    );

                    this.AddObjects(sheet, 2, input.TableDataList,
                        s => s.SummaryDate,
                        s => s.MachineName,
                        s => s.Yield);

                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}