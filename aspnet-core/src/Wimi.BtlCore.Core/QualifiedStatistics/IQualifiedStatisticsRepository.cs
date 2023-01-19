using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.QualifiedStatistics
{
    public interface IQualifiedStatisticsRepository : ITransientDependency
    {
        Task<List<QualificationData>> ListQualification(EnumStatisticalWays statisticalWay, string startTime,string endTime, List<int> deviceGroupId, List<int> shiftSolutionId, List<string> unionTables);
    }
}
