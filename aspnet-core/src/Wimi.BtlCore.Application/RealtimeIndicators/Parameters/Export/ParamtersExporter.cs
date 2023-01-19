using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;

namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Export
{
    public class ParamtersExporter : EpPlusExcelExporterBase, IParamtersExporter
    {

        public FileDto ExportToFile(GetHistoryParamtersListExportDto input)
        {
            var fileName = $"{input.MachineName}_{this.L("HistoryParameters")}_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(input.MachineName);

                    var columnIndex = 1;

                    foreach (var item in input.ParamList)
                    {
                        sheet.Cells[1, columnIndex].Value = item.Name;
                        sheet.Cells[1, columnIndex].Style.Font.Bold = true;

                        columnIndex++;
                    }

                    var rowIndex = 2;
                    foreach (var item in input.Parameters)
                    {
                        columnIndex = 1;

                        foreach (var column in input.ParamList)
                        {
                            var target = item.FirstOrDefault(s => s.Name.Equals(column.Code, StringComparison.OrdinalIgnoreCase));

                            if (!string.IsNullOrEmpty(target.Name))
                            {
                                var value = column.Code.Equals("creationtime", StringComparison.OrdinalIgnoreCase) ? target.Value.ToString().FormartMongoDateTimeWithMillisecond() : $"{target.Value} {column.Unit}";

                                if (target.Value.IsValidDateTime)
                                {
                                    sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss:ffff";
                                }

                                sheet.Cells[rowIndex, columnIndex].Value = value;
                            }
                            else
                            {
                                sheet.Cells[rowIndex, columnIndex].Value = "--";
                            }

                            columnIndex++;
                        }

                        rowIndex++;
                    }
                });
        }
    }
}
