namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 装卸刀操作枚举
    /// </summary>
    public enum EnumOperationType
    {
        /// <summary>
        /// The unload.
        /// </summary>
        [Display(Name = "卸刀")]
        Unload = 0, // 卸刀

        /// <summary>
        /// The load.
        /// </summary>
        [Display(Name = "装刀")]
        Load = 1 // 装刀
    }
}