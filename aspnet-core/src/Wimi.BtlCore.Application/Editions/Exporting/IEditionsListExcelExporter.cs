using System.Collections.Generic;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Editions.Dto;

namespace Wimi.BtlCore.Editions.Exporting
{
    public interface IEditionsListExcelExporter
    {
        FileDto ExportToFile(List<EditionListDto> editionListDtos);
    }
}