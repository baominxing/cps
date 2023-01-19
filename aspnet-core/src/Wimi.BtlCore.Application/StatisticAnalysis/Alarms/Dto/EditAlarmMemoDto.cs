namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using System;

    using Abp.Domain.Entities;

    public class EditAlarmMemoDto : Entity<long>
    {
        public string Code { get; set; }

        /// <summary>
        ///     持续时长（秒）
        /// </summary>
        public decimal Duration { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        public string Memo { get; set; }

        public string Message { get; set; }

        /// <summary>
        ///     开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
    }
}