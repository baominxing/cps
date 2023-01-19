using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Trace.Repository.Dtos;

namespace Wimi.BtlCore.Trace.Repository
{
    public interface ITraceRepository: ITransientDependency
    {
        Task<List<TraceFlowRecord>> QueryTraceFlowRecord(string partNo,string archivedTable);

        Task<PartDetail> QueryPartDetail(string partNo, string archivedTable);

        Task<List<TraceCatalogDto>> QueryTraceCatalog(QueryTraceCatalogInputDto input, int skipCount, int length);

        Task<int> QueryTraceCatalogCount(QueryTraceCatalogInputDto input);

        Task<IEnumerable<TraceExportItem>> ListTraceExportItem(QueryTraceCatalogInputDto input);

        Task<List<NgPartsResultDto>> QueryNgPart(string partNo, DateTime? startTime, DateTime? endTime, int length, int skipCount);

        Task<int> QueryNgPartCount(string partNo, DateTime? startTime, DateTime? endTime);

        Task<List<NGPartsExportDto>> ListNgPartsForExport(string partNo, DateTime? startTime, DateTime? endTime);
    }
}
