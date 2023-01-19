using Abp.Collections.Extensions;
using Abp.Dependency;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Wimi.BtlCore.AppSystem.Net.MimeTypes;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.DataExporting.Excel.EpPlus
{
    public abstract class EpPlusExcelExporterBase : BtlCoreDomainServiceBase, ITransientDependency
    {
        public IBtlFolders AppFolders { get; set; }

        protected void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                this.AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        protected void AddHeaderWithMerge(ExcelWorksheet sheet, string mergeColumn, int rowIndex, int columnIndex, string headerText)
        {
            sheet.Cells["A1:" + mergeColumn + ""].Merge = true;
            sheet.Cells[rowIndex, columnIndex].Value = headerText;
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = true;
            sheet.Cells[rowIndex, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 1].Style.Font.Size = 16;
        }

        protected void AddHeaderWithColor(ExcelWorksheet sheet, int row, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                sheet.Cells[row, i + 1].Value = headerTexts[i];
                sheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                sheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        protected void AddObjectsWithHyperlink<T>(
           ExcelWorksheet sheet,
           int startRowIndex,
           IList<T> items,
           params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                sheet.Cells[i + startRowIndex, 1].Value = i + 1;
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    int column = j + 1;
                    if (propertySelectors[j](items[i]) is DateTime)
                    {
                        sheet.Cells[i + startRowIndex, column + 1].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    }
                    sheet.Cells[i + startRowIndex, column + 1].Value = propertySelectors[j](items[i]);

                }
                sheet.Cells[i + startRowIndex, propertySelectors.Length + 1].Hyperlink = new ExcelHyperLink("#'" + propertySelectors[1](items[i]) + "'!A1", "查看");
                sheet.Cells[i + startRowIndex, propertySelectors.Length + 1].Style.Font.Color.SetColor(Color.DodgerBlue);
            }
        }

        protected void AddVerticalHeader(ExcelWorksheet sheet, int startIndex, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                this.AddVerticalObject(sheet, startIndex + i, headerTexts[i], true);
            }
        }

        protected void AddVerticalObject(ExcelWorksheet sheet, int rowIndex, string headerText, bool bold = false)
        {
            sheet.Cells[rowIndex, 1].Value = headerText;
            sheet.Cells[rowIndex, 1].Style.Font.Bold = bold;
        }

        protected void AddVerticalObjects<T>(
            ExcelWorksheet sheet,
            int startRowIndex,
            IList<T> items,
            int columnIndex,
            params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < propertySelectors.Length; i++)
            {
                for (var j = 0; j < items.Count; j++)
                {
                    if (propertySelectors[i](items[j]) is DateTime)
                    {
                        sheet.Cells[startRowIndex + i, columnIndex + j].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    }

                    sheet.Cells[startRowIndex + i, columnIndex + j].Value = propertySelectors[i](items[j]);
                }
            }
        }
        protected void AddObjects<T>(
            ExcelWorksheet sheet, 
            int startRowIndex, 
            IList<T> items, 
            params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    if (propertySelectors[j](items[i]) is DateTime)
                    {
                        sheet.Cells[i + startRowIndex, j + 1].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                    }

                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                this.Save(excelPackage, file);
            }

            return file;
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            var filePath = Path.Combine(this.AppFolders.TempFileDownloadFolder, file.FileToken);
            excelPackage.SaveAs(new FileInfo(filePath));
        }
    }
}