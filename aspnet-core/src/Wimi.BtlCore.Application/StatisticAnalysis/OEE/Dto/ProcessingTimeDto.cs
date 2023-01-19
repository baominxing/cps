namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    public class ProcessingTimeDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal PerfectTime { get; set; }

        public decimal ActualTime { get; set; }
    }
}