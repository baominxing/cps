using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    [Table("FmsCutterExtends")]
    public class FmsCutterExtend : Entity
    {
        [Comment("刀具Id")]
        public int FmsCutterId { get; set; }

        [JsonIgnore]
        [Comment("刀具")]
        public virtual FmsCutter FmsCutter { get; set; }

        [JsonIgnore]
        [Comment("自定义字段")]
        public virtual CustomField CustomField { get; set; }

        [Comment("自定义字段Id")]
        public int CustomFieldId { get; set; }

        [Comment("字段值")]
        public string FieldValue { get; set; }

    }
}
