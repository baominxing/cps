using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Cartons
{
    public enum EnumRuleType
    {
        [Display(Name = "Ascii")]
        Ascii = 0,

        [Display(Name = "固定字符串")]
        FixedString = 1,

        [Display(Name = "年")]
        Year = 2,

        [Display(Name = "月")]
        Month = 3,

        [Display(Name = "日")]
        Day = 4,

        [Display(Name = "季度")]
        Quarter = 5,

        [Display(Name = "周")]
        Week = 6,

        [Display(Name = "班次")]
        Shift = 7,

        [Display(Name = "产线")]
        Line = 8,

        [Display(Name = "流水号")]
        SerialNumber = 9,

        [Display(Name = "效验码")]
        CalibratorCode = 10,

        [Display(Name = "特殊码")]
        SpecialCode = 11,

        [Display(Name = "时间")]
        Time = 12
    }
}
