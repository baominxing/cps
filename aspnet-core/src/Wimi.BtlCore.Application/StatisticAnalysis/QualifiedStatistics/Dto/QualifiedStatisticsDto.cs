using System.Collections.Generic;
using Wimi.BtlCore.QualifiedStatistics;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto
{
    public class QualifiedStatisticsDto
    {
        public QualifiedStatisticsDto()
        {
            this.Items = new List<QualificationData>();
        }

        public string DisplayName { get; set; }

        public IEnumerable<QualificationData> Items { get; set; }
    }
}