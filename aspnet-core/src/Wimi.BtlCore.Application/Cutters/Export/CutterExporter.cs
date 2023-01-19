using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OfficeOpenXml.Style;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.Cutters.Export
{
    public class CutterExporter : EpPlusExcelExporterBase, ICutterExporter
    {
        public FileDto ExportToFile(List<CutterLoadAndUnloadRecordDto> input, DateTime? starTime, DateTime? endTime, QueryCutterRecordDto searchParam)
        {
            var fileName = $"换刀记录报表_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    this.AddHeaderWithMerge(sheet, "N1", 1, 1, "换刀记录报表");

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
                        this.L("CutterNo"),
                        this.L("CutterType"),
                        this.L("MachineName"),
                        this.L("EquipmentCutterLocation"),
                        this.L("LifeCountingTypes"),
                        this.L("RawLife"),
                        this.L("UsedLife"),
                        this.L("RestLife"),
                        this.L("OperationType"),
                        this.L("Creator"),
                        this.L("CreationTime"),
                        this.L("Operator"),
                        this.L("OperationTime"));

                    this.AddObjects(
                        sheet,
                        4,
                        input,
                        _ => "",
                        _ => _.CutterNo,
                        _ => _.CutterModelName,
                        _ => _.MachineName,
                        _ => _.CutterTValue.HasValue ? _.CutterTValue.Value.ToString() : "",
                        _ => _.CountingMethodName,
                        _ => _.OriginalLife,
                        _ => _.UsedLife,
                        _ => _.RestLife,
                        _ => _.OperationTypeName,
                        _ => _.CreatorUserName,
                        _ => _.CreationTime.ToString("yyyy-MM-dd HH:mm"),
                        _ => _.Operator,
                        _ => _.OperatorTime.HasValue ? _.OperatorTime.Value.ToString("yyyy-MM-dd HH:mm") : "");



                    //设置边框
                    for (int i = 1; i <= 14; i++)
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

        public FileDto ExportToFile(List<CutterStatesDto> input, QueryCutterStateDto searchParam)
        {
            var fileName = $"刀具状态报表_{DateTime.Now.ToMongoDateTime()}.xlsx";

            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    this.AddHeaderWithMerge(sheet, "N1", 1, 1, "刀具状态报表");

                    this.AddHeaderWithColor(
                        sheet,
                        3,
                        "序号",
                        this.L("CutterNo"),
                        this.L("ToolType"),
                        this.L("CutterType"),
                        this.L("MachineCode"),
                        this.L("MachineName"),
                        this.L("CutterTValue"),
                        this.L("CutterUsedStatus"),
                        this.L("CutterLifeStatus"),
                        this.L("CountingMethod"),
                        this.L("OriginalLife"),
                        this.L("UsedLife"),
                        this.L("RestLife"),
                        this.L("WarningLife"));

                    this.AddObjects(
                        sheet,
                        4,
                        input,
                        _ => "",
                        _ => _.CutterNo,
                        _ => _.CutterTypeName,
                        _ => _.CutterModelName,
                        _ => _.MachineNo,
                        _ => _.MachineName,
                        _ => _.CutterTValue.HasValue ? _.CutterTValue.Value.ToString() : "",
                        _ => this.L(_.UsedStatusName),
                        _ => this.L(_.LifeStatusName),
                        _ => this.L(_.CountingMethodName),
                        _ => _.OriginalLife,
                        _ => _.UsedLife,
                        _ => _.RestLife,
                        _ => _.WarningLife);

                    //设置边框
                    for (int i = 1; i <= 14; i++)
                    {
                        for (int n = 1; n <= input.Count + 3; n++)
                        {
                            if (n > 3)
                            {
                                sheet.Cells[n, 1].Value = n - 3;
                                //修改背景色
                                if (sheet.Cells[n, 9].Value.ToString() == "报警")
                                {
                                    sheet.Cells[n, 1, n, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[n, 1, n, 14].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                }
                                else if (sheet.Cells[n, 9].Value.ToString() == "警告")
                                {
                                    sheet.Cells[n, 1, n, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[n, 1, n, 14].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                }
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
