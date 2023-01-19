namespace Wimi.BtlCore.CommonEnums
{
    using System.ComponentModel.DataAnnotations;

    public enum EnumImportTypes
    {
        /// <summary>
        /// The users.
        /// </summary>
        [Display(Name = "用户")]
        Users, 

        /// <summary>
        /// The machines.
        /// </summary>
        [Display(Name = "设备")]
        Machines,

        /// <summary>
        /// 采集变量
        /// </summary>
        [Display(Name = "采集变量")]
        GatherParams


    }
}