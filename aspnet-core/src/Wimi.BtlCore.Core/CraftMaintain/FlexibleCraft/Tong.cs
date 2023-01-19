using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wimi.BtlCore.CraftMaintain
{
    [Table("Tongs")]
    public class Tong : FullAuditedEntity
    {
        /// <summary>
        /// 夹具编号
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Comment("编号")]
        public string Code { get; set; }

        /// <summary>
        /// 夹具名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Comment("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        [Required]
        [Comment("容量")]
        public int Capacity { get; set; }

        /// <summary>
        /// 程序号A-F
        /// </summary>
        [MaxLength(40)]
        [Comment("程序号A")]
        public string ProgramA { get; set; }

        [MaxLength(40)]
        [Comment("程序号B")]
        public string ProgramB { get; set; }

        [MaxLength(40)]
        [Comment("程序号C")]
        public string ProgramC { get; set; }

        [MaxLength(40)]
        [Comment("程序号D")]
        public string ProgramD { get; set; }

        [MaxLength(40)]
        [Comment("程序号E")]
        public string ProgramE { get; set; }

        [MaxLength(40)]
        [Comment("程序号F")]
        public string ProgramF { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        [Comment("备注")]
        public string Note { get; set; }

        public Dictionary<string,string> GetPrograms()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(ProgramA))
            {
                list["A"] = ProgramA;
            }
            if (!string.IsNullOrWhiteSpace(ProgramB))
            {
                list["B"] = ProgramB;
            }
            if (!string.IsNullOrWhiteSpace(ProgramC))
            {
                list["C"] = ProgramC;
            }
            if (!string.IsNullOrWhiteSpace(ProgramD))
            {
                list["D"] = ProgramD;
            }
            if (!string.IsNullOrWhiteSpace(ProgramE))
            {
                list["E"] = ProgramE;
            }
            if (!string.IsNullOrWhiteSpace(ProgramF))
            {
                list["F"] = ProgramF;
            }
            return list;
        }

        public string ToProgramString()
        { 
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ProgramA))
            {
                sb.Append($" <b> A:</b> <span class=\"lable-primary\">{ProgramA}</span>");
            }
            if (!string.IsNullOrWhiteSpace(ProgramB))
            {
                sb.Append($" <b> B:</b> <span class=\"lable-secondary\">{ProgramB}</span>");
            }
            if (!string.IsNullOrWhiteSpace(ProgramC))
            {
                sb.Append($" <b> C:</b> <span class=\"lable-success\">{ProgramC}</span>");
            }
            if (!string.IsNullOrWhiteSpace(ProgramD))
            {
                sb.Append($" <b> D:</b> <span class=\"lable-danger\">{ProgramD}</span>");
            }
            if (!string.IsNullOrWhiteSpace(ProgramE))
            {
                sb.Append($" <b> E:</b> <span class=\"lable-warning\">{ProgramE}</span>");
            }
            if (!string.IsNullOrWhiteSpace(ProgramF))
            {
                sb.Append($" <b> F:</b> <span class=\"lable-info\">{ProgramF}</span>");
            }
            return sb.ToString();
        }
    }
}
