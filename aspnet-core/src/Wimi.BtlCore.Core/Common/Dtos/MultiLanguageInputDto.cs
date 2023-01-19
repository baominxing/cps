using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Common.Dtos
{
    public class MultiLanguageInputDto : EntityDto
    {
        public int SheetIndex { get; set; }
        //code所在列的index
        public int CodeIndex { get; set; }
        //翻译所在列的index
        public int ValueIndex { get; set; }
        //是否去除表头行
        public bool IsCheck { get; set; }
        public string Culture { get; set; }
    }
}
