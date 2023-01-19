using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    public enum EnumFmsCutterCountType
    {
        [Display(Name = "次数")]
        Degree = 0,

        [Display(Name = "时间")]
        Time = 1,

        [Display(Name = "切削长度")]
        UseLength = 2
    }
}
