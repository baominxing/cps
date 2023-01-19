using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    public enum EnumFmsUseType
    {
        [Display(Name = "未使用")]
        Unused = 0,

        [Display(Name = "正常")]
        Normal = 1,

        [Display(Name = "预警")]
        Warning = 2
    }
}
