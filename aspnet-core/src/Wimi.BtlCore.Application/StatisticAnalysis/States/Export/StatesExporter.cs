using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.StatisticAnalysis.States.Export
{
    public class StatesExporter : EpPlusExcelExporterBase, IStatesExporter
    {
        public FileDto ExportToFile(IEnumerable<MachineStateRateDto> input)
        {
            var fileName = $"{this.L("TimeStatistics")}_{DateTime.Now.ToMongoDateTime()}.xlsx";
            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("TimeStatistics"));

                    this.AddHeader(
                            sheet,
                            this.L("StatisticalMethod"),
                            this.L("Machines"),
                            this.L("Stop"),
                            this.L("Run"),
                            this.L("Free"),
                            this.L("Offline"),
                            this.L("Debug")
                        );


                    this.AddObjects(sheet, 2, input.ToList(),
                      s => s.SummaryDate,
                      s => s.SummaryName,
                      s => (s.StopDurationRate * 100).ToString("#0.00")+"%",
                      s => (s.RunDurationRate * 100).ToString("#0.00") + "%",
                      s => (s.FreeDurationRate * 100).ToString("#0.00") + "%",
                      s => (s.OfflineDurationRate * 100).ToString("#0.00") + "%",
                      s => (s.DebugDurationRate * 100).ToString("#0.00") + "%"
                      );

                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}
