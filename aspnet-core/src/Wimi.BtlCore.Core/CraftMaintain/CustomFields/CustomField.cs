using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.FmsCutters
{

    [Table("CustomFields")]
    public class CustomField : Entity
    {
        [Required]
        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("编号")]
        public string Code { get; set; }

        [Comment("显示类型")]
        public EnumCustomDisplayType DisplayType { get; set; }

        [Comment("是否必需")]
        public bool IsRequired { get; set; }

        [Comment("最大长度")]
        public int MaxLength { get; set; }

        /// <summary>
        /// 编辑节点
        /// </summary>
        [Comment("编辑节点")]
        public string HtmlTemplate { get; set; }

        /// <summary>
        /// 渲染Dom节点
        /// </summary>
        [Comment("渲染Dom节点")]
        public string RenderHtml { get; set; }

        [Comment("标记")]
        public string Remark { get; set; }

        public static string GetRandomCode()
        {
            return DateTime.Now.ToString("yyyyMMdd");// new StringCreator().Get(8);
        }
    }


    public enum EnumCustomDisplayType
    {
        [Display(Name = "文本框")]
        SingleLineTextBox = 1,

        [Display(Name = "单选下拉框")]
        SingleDropList = 2,

        [Display(Name = "复选框")]
        CheckBox = 3,

        [Display(Name = "单选按钮")]
        Radio = 4
    }
}