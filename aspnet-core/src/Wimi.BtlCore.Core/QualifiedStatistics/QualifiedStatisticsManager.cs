using System.Collections.Generic;
using System.Linq;

namespace Wimi.BtlCore.QualifiedStatistics
{
    public class QualifiedStatisticsManager: BtlCoreDomainServiceBase,IQualifiedStatisticsManager
    {

        public List<QualificationData> CrossedResult(List<QualificationData> originalQueries,IEnumerable<QualificationData> crossedQueries)
        {

            var crossedResult = from cq in crossedQueries
                                join oq in originalQueries on new { cq.SummaryDate } equals new { oq.SummaryDate } into leftQueries
                                from lq in leftQueries.DefaultIfEmpty()
                                select new QualificationData
                                {
                                    SummaryDate = cq.SummaryDate,
                                    //QualifiedTableRate = lq == null ? "0%" : (lq.QualifiedOfflineCount + lq.NgCount) == 0 ? "0%" : decimal.Round(lq.QualifiedOfflineCount / (lq.QualifiedOfflineCount + lq.NgCount) * 100, 2) + "%",
                                    ProcessingCount = lq?.ProcessingCount ?? 0,
                                    OnlineCount = lq?.OnlineCount ?? 0,
                                    QualifiedOfflineCount = lq?.QualifiedOfflineCount ?? 0,
                                    NgCount = lq?.NgCount ?? 0
                                };
            return crossedResult.ToList();
        }

        public List<QualificationData> EchartsResult(List<QualificationData> originalQueries, IEnumerable<QualificationData> crossedQueries)
        {
            var echartsResult = from cq in crossedQueries
                join oq in originalQueries on new { cq.SummaryDate } equals new { oq.SummaryDate } into leftQueries
                from lq in leftQueries.DefaultIfEmpty()
                select new QualificationData
                {
                    SummaryDate = cq.SummaryDate,
                    //QualifiedEchartRate = lq == null ? 0 : (lq.QualifiedOfflineCount + lq.NgCount) == 0 ? 0 : decimal.Round(lq.QualifiedOfflineCount / (lq.QualifiedOfflineCount + lq.NgCount) * 100, 2)
                };
            return echartsResult.ToList();
        }

        public IEnumerable<QualificationData> CrossedQueries(List<QualificationData> originalQueries)
        {
            var summaryDateQueries = from q in originalQueries
                group q.SummaryDate
                    by q.SummaryDate
                into qg
                select new { SummaryDate = qg.Key };

         return from dt in summaryDateQueries
                select new QualificationData
                {
                    SummaryDate = dt.SummaryDate,
                };
        }
    }
}
