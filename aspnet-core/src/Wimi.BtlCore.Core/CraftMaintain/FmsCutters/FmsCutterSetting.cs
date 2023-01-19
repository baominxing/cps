using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.FmsCutters
{
    [Table("FmsCutterSettings")]
    public class FmsCutterSetting : CreationAuditedEntity
    {
        public FmsCutterSetting()
        {

        }

        public FmsCutterSetting(string code, int seq, EnumFieldType type = EnumFieldType.Basics, bool isShow = true)
        {
            this.Code = code;
            this.Seq = seq;
            this.Type = type;
            this.IsShow = isShow;
        }

        [Comment("编号")]
        public string Code { get; set; }

        [Comment("顺序")]
        public int Seq { get; set; }

        [Comment("字段类型")]
        public EnumFieldType Type { get; set; }

        [Comment("是否显示")]
        public bool IsShow { get; set; }
    }

    public enum EnumFieldType
    {
        [Display(Name = "基础字段")]
        Basics = 0,

        [Display(Name = "拓展字段")]
        Extend = 1
    }
}
