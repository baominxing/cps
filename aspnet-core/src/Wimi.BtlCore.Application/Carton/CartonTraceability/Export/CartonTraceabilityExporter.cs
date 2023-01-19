using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml.Style;
using Wimi.BtlCore.Carton.CartonTraceability.Dtos;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.Carton.CartonTraceability.Export
{
    public class CartonTraceabilityExporter : EpPlusExcelExporterBase, ICartonTraceabilityExporter
    {
        public FileDto ExportCartonToFile(List<CartonExportDto> input)
        {
            var fileName = $"装箱追溯报表_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("装箱追溯报表");
                    sheet.OutLineApplyStyle = true;
                    this.AddHeader(sheet,
                        "箱码",
                        "设备组",
                        "最大包装数",
                        "实际包装数",
                        "打印标签次数",
                        "操作时间",
                        "工件编号",
                        "装箱日期",
                        "班次",
                        "操作时间"
                    );

                    // 从第二行开始填写数据， 垂直填写
                    var rowNo = 2;
                    for (var i = 0; i < input.Count(); i++)
                    {
                        var item = input[i];

                        var propertySelectors = new string[]
                        {
                            item.CartonNo,
                            item.DeviceGroupName,
                            item.MaxPackingCount.ToString(),
                            item.RealPackingCount.ToString(),
                            item.PrintLabelCount.ToString(),
                            item.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        // 合并单元格
                        for (var j = 1; j <= 6; j++)
                        {
                            var cell = sheet.Cells[rowNo, j, rowNo + item.Details.Count - 1, j];

                            cell.Merge = true;
                            cell.Value = propertySelectors[j - 1];
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // 填写明细信息
                        // 7列开始， 10列结束
                        for (var j = 0; j < item.Details.Count(); j++)
                        {
                            var key = item.Details[j];

                            var detailSelectors = new string[]
                            {
                                key.PartNo,
                                key.ShiftDay.ToString("yyyy-MM-dd"),
                                key.ShiftSolutionItemName,
                                key.OperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                            };

                            for (var k = 7; k <= 10; k++)
                            {
                                sheet.Cells[rowNo + j, k].Value = detailSelectors[k - 7];
                                sheet.Cells[rowNo + j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }
                        }

                        // 切换到下一行
                        rowNo += item.Details.Count;
                    }

                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}
