namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumCountingMethod
    {
        /// <summary>
        /// The number.
        /// </summary>
        [Display(Name = "次数")]
        Number = 0, 

        /// <summary>
        /// 时间统计.
        /// </summary>
        [Display(Name = "时间")]
        Time = 1
    }
}