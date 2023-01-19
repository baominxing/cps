namespace Wimi.BtlCore.StaffPerformance
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumDeviceUseState
    {
        /// <summary>
        /// The all.
        /// </summary>
        [Display(Name = "所有")]
        All = 0, 

        /// <summary>
        /// The online.
        /// </summary>
        [Display(Name = "上线")]
        Online = 1, 

        /// <summary>
        /// The offline.
        /// </summary>
        [Display(Name = "下线")]
        Offline = 2, 

        /// <summary>
        /// The mine.
        /// </summary>
        [Display(Name = "我的")]
        Mine = 3
    }
}