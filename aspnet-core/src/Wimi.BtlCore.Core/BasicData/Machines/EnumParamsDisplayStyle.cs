using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.BasicData.Machines
{
    /// <summary>
    ///     参数显示方式
    /// </summary>
    public enum EnumParamsDisplayStyle
    {
        /// <summary>
        ///     折线图
        /// </summary>
        [Display(Name = "线性趋势")]
        LineChart = 0,

        /// <summary>
        ///     块状图
        /// </summary>
        [Display(Name = "块状面板")]
        BlockChart = 1,

        ///// <summary>
        /////     块状图
        ///// </summary>
        // [Display(Name = "动态面板")]
        // DynamicPanel = 2,

        /// <summary>
        ///     仪表盘
        /// </summary>
        [Display(Name = "仪表盘")]
        GaugePanel = 3
    }
}
