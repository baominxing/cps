using System.Collections.Generic;

using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Editions.Dto;

namespace Wimi.BtlCore.Editions.Exporting
{
    internal class EditionsListExcelExporter : EpPlusExcelExporterBase, IEditionsListExcelExporter
    {
        private readonly IAbpSession abpSession;

        private readonly ITimeZoneConverter timeZoneConverter;

        public EditionsListExcelExporter(ITimeZoneConverter timeZoneConverter, IAbpSession abpSession)
        {
            this.abpSession = abpSession;
            this.timeZoneConverter = timeZoneConverter;
        }

        public FileDto ExportToFile(List<EditionListDto> editionListDtos)
        {
            return this.CreateExcelPackage(
                "EditionsList.xlsx",
                excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(this.L("Edition"));
                        sheet.OutLineApplyStyle = true;

                        this.AddHeader(sheet, this.L("EditionName"), this.L("Name"), this.L("CreationTime"));

                        this.AddObjects(
                            sheet,
                            2,
                            editionListDtos,
                            _ => _.DisplayName,
                            _ => _.Name,
                            _ =>
                            this.timeZoneConverter.Convert(
                                _.CreationTime,
                                this.abpSession.TenantId,
                                this.abpSession.GetUserId()));

                        // Formatting cells
                        var creationTimeColumn = sheet.Column(3);
                        creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                        for (var i = 1; i <= 3; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
        }
    }
}