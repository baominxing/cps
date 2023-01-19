namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 刀具寿命状态
    /// </summary>
    public enum EnumCutterLifeStates
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Display(Name = "正常")]
        Normal = 1,

        /// <summary>
        /// 报警
        /// </summary>
        [Display(Name = "报警")]
        Alarm = 2,

        /// <summary>
        /// 警告
        /// </summary>
        [Display(Name = "警告")]
        Warning = 3
    }
}