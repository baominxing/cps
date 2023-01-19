namespace Wimi.BtlCore.BasicData.Dto
{
    using OfficeOpenXml;

    public class ImportDataInputDto
    {
        public ExcelWorksheet ExcelWorksheet { get; set; }

        public string Type { get; set; }
    }
}