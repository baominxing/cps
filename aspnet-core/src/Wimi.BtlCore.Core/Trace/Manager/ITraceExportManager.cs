using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Trace.Dto;

namespace Wimi.BtlCore.Trace.Manager
{
    public interface ITraceExportManager : IDomainService
    {
        Task<List<TraceExportDto>> ListTraceCatalogForExport(TraceCatalogsInputDto input);

        List<TraceExportDto> ListTraceRecordByPartNoForExport(string partNo);

        Task<IEnumerable<TraceExportItem>> ListTraceExportItem(TraceCatalogsInputDto input);
    }
}
