namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    public class ProductPlanYieldDto
    {
        public string ProductName { get; set; }

        public int? PlanId { get; set; }

        public string PlanName { get; set; }

        public int PlanAmount { get; set; }

        public int CompletedAmount { get; set; }

        public string CompleteRate { get; set; }

        public int ShiftAmount { get; set; }

        public int ShiftDayAmount { get; set; }

        public int ShiftWeekAmount { get; set; }

        public int ShiftMonthAmount { get; set; }


        public int ProductId { get; set; }

        public string StatisticalWay { get; set; }

        public int StatisticalWayAmount { get; set; }

        public int DeviceGroupId { get; set; }


    }
}
