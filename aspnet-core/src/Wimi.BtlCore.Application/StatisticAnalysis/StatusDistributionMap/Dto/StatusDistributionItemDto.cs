using System;
using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class StatusDistributionItemDto
    {
        public long StateId { get; set; }

        [MaxLength(200)]
        public string Memo { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string StateName { get; set; }

        public string Hexcode { get; set; }
        public string StateCode { get; set; }
    }
}