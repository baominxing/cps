using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    public enum EnumFmsCutterState
    {
        [Display(Name = "未激活")]
        Unactivated = 0,

        [Display(Name = "正常使用")]
        Normal = 1,

        [Display(Name = "异常禁用")]
        Disabled = 2
    }
}
