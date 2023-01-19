using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Traceability.Dto;

namespace Wimi.BtlCore.Traceability.Export
{
    public class TraceExporter : EpPlusExcelExporterBase, ITraceExporter
    {
        public FileDto ExportToFile(List<TraceExportItemDto> input)
        {
            var fileName = $"追溯报表_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("追溯报表");
                    sheet.OutLineApplyStyle = true;
                    this.AddHeader(sheet,
                        "工件编码",
                        "状态",
                        "是否合格",
                        "班次信息",
                        "流程编号",
                        "设备名称",
                        "进入时间",
                        "离开时间",
                        "流程状态"
                    );

                    // 从第二行开始填写数据， 垂直填写
                    var rowNo = 2;
                    for (var i = 0; i < input.Count(); i++)
                    {
                        var item = input[i];

                        var propertySelectors = new[]
                        {
                            item.PartNo,
                            item.TraceStates,
                            item.Qualified ==null?"未知":item.Qualified.Value?"合格":"不合格",
                            item.ShiftItemName
                        };

                        // 合并单元格
                        for (var j = 1; j <= 4; j++)
                        {
                            var cell = sheet.Cells[rowNo, j, rowNo + item.Length - 1, j];

                            cell.Merge = true;
                            cell.Value = propertySelectors[j - 1];
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // 填写明细信息
                        // 5列开始， 9列结束
                        for (var j = 0; j < item.DetailItems.Count(); j++)
                        {
                            var key = item.DetailItems[j];

                            var detailSelectors = new[]
                            {
                                key.FlowName,
                                key.MachineName,
                                key.EntryTime,
                                key.LeftTime,
                                key.State == "1" ? "完成" : "进行中"
                            };

                            for (var k = 5; k <= 9; k++)
                            {
                                sheet.Cells[rowNo + j, k].Value = detailSelectors[k - 5];
                                sheet.Cells[rowNo + j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }
                        }

                        // 切换到下一行
                        rowNo = rowNo + item.Length;
                    }

                    sheet.Cells.AutoFitColumns();
                });
        }

        public FileDto ExportToFile(List<NGPartsExportDto> input, DateTime? starTime, DateTime? endTime, NgPartsRequestDto searchParam)
        {
            var fileName = $"NG工件报表_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    this.AddHeaderWithMerge(sheet, "J1", 1, 1, "NG工件报表");

                    sheet.Cells[2, 1].Value = "开始时间";
                    sheet.Cells[2, 1].Style.Font.Bold = true;
                    sheet.Cells[2, 2].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    sheet.Cells[2, 2].Value = starTime;

                    sheet.Cells[2, 4].Value = "结束时间";
                    sheet.Cells[2, 4].Style.Font.Bold = true;
                    sheet.Cells[2, 5].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    sheet.Cells[2, 5].Value = endTime;

                    this.AddHeaderWithColor(
                        sheet,
                        3,
                        "序号",
                        this.L("DeviceGroup"),
                        this.L("WorkpieceQRCode"),
                        this.L("OnlineTime"),
                        this.L("OfflineTime"),
                        this.L("MachineName"),
                        this.L("StationName"),
                        this.L("Shift"),
                        this.L("State"),
                        this.L("DefectiveReasonNames"));

                    this.AddObjects(
                        sheet,
                        4,
                        input,
                        _ => "",
                        _ => _.DeviceGroupName,
                        _ => _.PartNo,
                        _ => _.OnlineTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        _ => _.OfflineTime.HasValue ? _.OfflineTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        _ => _.MachineName,
                        _ => _.StationName,
                        _ => _.ShiftName,
                        _ => _.State,
                        _ => _.DefectiveReasonNames);

                    //设置边框
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int n = 1; n <= input.Count + 3; n++)
                        {
                            if (n > 3)
                            {
                                sheet.Cells[n, 1].Value = n - 3;
                            }
                            sheet.Cells[n, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            sheet.Cells[n, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            sheet.Cells[n, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            sheet.Cells[n, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }
                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }

    }
}
