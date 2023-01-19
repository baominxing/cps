using System;

namespace Wimi.BtlCore.Order.DefectiveStatisticses.Dtos
{
    public class DefectiveStatisticsDto
    {
        public int Count { get; set; }

        public DateTime CreationTime { get; set; }

        public string DefectiveReasonCode { get; set; }

        public string DefectiveReasonName { get; set; }

        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string ProductCode { get; set; }

        public int ProductId { get; set; }

        public string ProductionPlanCode { get; set; }

        public string ProductName { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string WorkOrderCode { get; set; }
    }
}
