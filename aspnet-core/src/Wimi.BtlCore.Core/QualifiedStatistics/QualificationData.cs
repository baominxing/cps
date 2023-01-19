using System;

namespace Wimi.BtlCore.QualifiedStatistics
{
    public class QualificationData
    {

        public string SummaryDate { get; set; }


        public string DisplayName { get; set; }

        /// <summary>
        /// 合格率
        /// </summary>
        public string QualifiedTableRate => $"{this.QualifiedEchartRate:p}";

        /// <summary>
        /// 合格下线数
        /// </summary>
        public decimal QualifiedOfflineCount { get; set; }

        /// <summary>
        /// 上线数
        /// </summary>
        public decimal OnlineCount { get; set; }

        /// <summary>
        /// 正在加工数
        /// </summary>
        public decimal ProcessingCount { get; set; }

        /// <summary>
        /// Ng数
        /// </summary>
        public decimal NgCount { get; set; }

        public decimal QualifiedEchartRate => this.OnlineCount == 0 || (this.OnlineCount - this.ProcessingCount) == 0 ? 0 : Math.Round(this.QualifiedOfflineCount / (this.OnlineCount - this.ProcessingCount), 4);
    }
}