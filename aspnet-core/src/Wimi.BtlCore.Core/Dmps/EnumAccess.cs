using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Dmps
{
    public enum EnumAccess
    {
        [Display(Name = "只读")]
        Read = 1,

        [Display(Name = "读写")]
        ReadWrite = 2
    }
}
