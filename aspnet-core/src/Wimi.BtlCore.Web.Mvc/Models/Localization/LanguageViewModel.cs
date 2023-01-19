using Abp.AutoMapper;
using Wimi.BtlCore.Common.Dtos;

namespace Wimi.BtlCore.Web.Models.Localization
{
    [AutoMapTo(typeof(MultiLanguageInputDto))]
    public class LanguageViewModel
    {
        public int SheetIndex { get; set; }

        //code所在列的index
        public int CodeIndex { get; set; }

        //翻译所在列的index
        public int ValueIndex { get; set; }

        //是否去除表头行
        public string TitleCheck { get; set; }

        public string Culture { get; set; }
    }
}
