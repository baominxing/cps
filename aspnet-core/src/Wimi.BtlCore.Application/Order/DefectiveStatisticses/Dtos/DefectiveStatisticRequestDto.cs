using System;

namespace Wimi.BtlCore.Order.DefectiveStatisticses.Dtos
{
    public class DefectiveStatisticRequestDto
    {
        public DateTime? EndTime { get; set; }

        public int? ProductId { get; set; }

        public DateTime? StartTime { get; set; }
    }
}
