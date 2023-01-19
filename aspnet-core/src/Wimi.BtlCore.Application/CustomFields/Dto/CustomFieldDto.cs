using Abp.AutoMapper;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CustomFields.Dto
{
    [AutoMap(typeof(CustomField))]
    public class CustomFieldDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public EnumCustomDisplayType DisplayType { get; set; }

        public bool IsRequired { get; set; }

        public int MaxLength { get; set; } 

        public string HtmlTemplate { get; set; }

        public string RenderHtml { get; set; }

        public string Remark { get; set; }
    }
}