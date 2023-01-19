using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using Wimi.BtlCore.Cutter.Dto;

namespace Wimi.BtlCore.Web.Models.Cutter.CutterType
{
    [AutoMap(typeof(CutterModelDto))]
    public class CutterModelModel : CutterModelDto
    {
        public CutterModelModel()
        {
            this.IsEditMode = false;
            this.IsCutterNoPrefixCanEdit = false;
        }

        public List<ComboboxItemDto> ValidStatusList { get; set; } = new List<ComboboxItemDto>();

        public bool IsCutterNoPrefixCanEdit { get; set; }

        public bool IsEditMode { get; set; }
    }
}
