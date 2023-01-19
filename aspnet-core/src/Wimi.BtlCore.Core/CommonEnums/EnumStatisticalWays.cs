namespace Wimi.BtlCore.CommonEnums
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumStatisticalWays
    {
        /// <summary>
        /// The by day.
        /// </summary>
        [Display(Name = "按天")]
        ByDay, 

        /// <summary>
        /// The by week.
        /// </summary>
        [Display(Name = "按周")]
        ByWeek, 

        /// <summary>
        /// The by month.
        /// </summary>
        [Display(Name = "按月")]
        ByMonth, 

        /// <summary>
        /// The by year.
        /// </summary>
        [Display(Name = "按年")]
        ByYear, 

        /// <summary>
        /// The by shift.
        /// </summary>
        [Display(Name = "按班次")]
        ByShift, 

        /// <summary>
        /// The by machine shift detail.
        /// </summary>
        [Display(Name = "按具体某个班次")]
        ByMachineShiftDetail
    }

    public enum EnumLookupWays
    {
        /// <summary>
        /// The by machine.
        /// </summary>
        [Display(Name = "按设备")]
        ByMachine, 

        /// <summary>
        /// The by date.
        /// </summary>
        [Display(Name = "按日期")]
        ByDate
    }
}