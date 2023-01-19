namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    public class ProcessPlanRateDto
    {
        public string PlanCode { get; set; }
        public string PlanName { get; set; }

        public string ProductName { get; set; }

        public int PlanAmount { get; set; }

        public int CompleteAmount { get; set; }

        public string TotalCompleteRate { get; set; }

        public string StatisticalWay { get; set; }

        public int StatisticalWayAmount { get; set; }

        public string SummaryDate { get; set; }

        public int SummaryDateAmount { get; set; }

        public string SummaryDateCompleteRate { get; set; }

        public int DeviceGroupId { get; set; }
    }
}
