using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Dmps
{
    public enum EnumDataType
    {
        [Display(Name = "位")]
        Bit = 0,
        [Display(Name = "字节")]
        Byte = 1,
        [Display(Name = "字")]
        Word = 2,
        [Display(Name = "双字")]
        DWord = 3,
        [Display(Name = "四字")]
        QWord = 4,
        [Display(Name = "有符号字节")]
        SByte = 5,
        [Display(Name = "有符号字")]
        SWord = 6,
        [Display(Name = "有符号双字")]
        SDWord = 7,
        [Display(Name = "有符号四字")]
        SQWord = 8,
        [Display(Name = "单精度浮点数")]
        Float = 9,
        [Display(Name = "双精度浮点数")]
        Double = 10,
        [Display(Name = "字符串")]
        String = 11,
        [Display(Name = "日期时间")]
        DateTime = 12,
        [Display(Name = "任意类型")]
        Any = 255,
        [Display(Name = "数组")]
        Array = 256,
        [Display(Name = "NULL")]
        Null = 512
    }
}
