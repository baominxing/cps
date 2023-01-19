namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 刀具使用状态
    /// </summary>
    public enum EnumCutterUsedStates
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Display(Name = "新增")]
        New = 1,

        /// <summary>
        /// 未装刀
        /// </summary>
        [Display(Name = "未装刀")]
        NotLoad = 2,

        /// <summary>
        /// 已装刀
        /// </summary>
        [Display(Name = "已装刀")]
        Loading = 3,

        /// <summary>
        /// 已拆卸
        /// </summary>
        [Display(Name = "已拆卸")]
        UnLoad = 4
    }
}