namespace Wimi.BtlCore.SummaryStatistics
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumEfficiencyTrendsGroupType
    {
        /// <summary>
        /// The alarms rate.
        /// </summary>
        [Display(Name = "故障率")]
        AlarmsRate = 0, 

        /// <summary>
        /// The using rate.
        /// </summary>
        [Display(Name = "开机率")]
        UsingRate = 1
    }
}