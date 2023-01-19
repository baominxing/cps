using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Export
{
    public class EfficiencyTrendsExporter : EpPlusExcelExporterBase, IEfficiencyTrendsExporter
    {
        public FileDto ExportToFile(GetEfficiencyTrendsListDto input)
        {
            var fileName = $"{input.GroupTypeName}_{DateTime.Now.ToMongoDateTime()}.xlsx";
            return this.CreateExcelPackage(
                fileName,
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(input.GroupTypeName);

                    var titles = input.EfficiencyTrendsColumns.Select(x => x.Title).ToArray();
                    var titleDatas = input.EfficiencyTrendsColumns.Select(x => x.Data).ToArray();

                    Func<Dictionary<string, string>, object>[] propertySelectors = new Func<Dictionary<string, string>, object>[titleDatas.Count()];

                    for (var i = 0; i < titleDatas.Count(); i++)
                    {
                        var item = titleDatas[i];
                        if(item.ToLower() == "dimensions")
                        {
                            item = "dimensions";
                        }
                        propertySelectors[i] = s => s[item];
                    }

                    this.AddHeader(
                            sheet,
                            titles
                        );


                    this.AddObjects(sheet, 2, input.EfficiencyTrendsData.Items.Select(x=>x.RateData).ToList(),
                       propertySelectors);

                    sheet.OutLineApplyStyle = true;
                    sheet.Cells.AutoFitColumns();
                });
        }
    }
}
