using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Cutter
{
    public enum EnumCutterLifeMethod
    {
        /// <summary>
        /// 刀具组件
        /// </summary>
        [Display(Name = "刀具组件")]
        ByComponent = 1,

        /// <summary>
        /// 工件计数
        /// </summary>
        [Display(Name = "工件计数")]
        ByCount = 2
    }
}
