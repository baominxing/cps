using Abp.Domain.Services;
using System.Collections.Generic;

namespace Wimi.BtlCore.QualifiedStatistics
{
    public interface IQualifiedStatisticsManager : IDomainService
    {
        List<QualificationData> CrossedResult(List<QualificationData> originalQueries,IEnumerable<QualificationData> crossedQueries);

        List<QualificationData> EchartsResult(List<QualificationData> originalQueries,IEnumerable<QualificationData> crossedQueries);

        IEnumerable<QualificationData> CrossedQueries(List<QualificationData> originalQueries);
    }
}
