namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto
{
    public class EfficiencyTrendasOutputListDto
    {
        /// <summary>
        ///     统计维度
        /// </summary>
        public string Dimensions { get; set; }

        /// <summary>
        ///     设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        ///     比率
        /// </summary>
        public decimal Rate { get; set; }
    }
}