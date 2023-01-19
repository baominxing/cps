using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Trace.Dto;

namespace Wimi.BtlCore.Trace
{
    public interface ITraceExportRepository : ITransientDependency
    {
        Task<IEnumerable<TraceExportItem>> ListTraceExportItem(TraceCatalogsInputDto input);
    }
}
