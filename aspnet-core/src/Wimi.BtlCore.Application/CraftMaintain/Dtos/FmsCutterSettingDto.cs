using System.Collections.Generic;
using Abp.AutoMapper;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
   
    public class FmsCutterSettingDto
    {
        public FmsCutterSettingDto()
        {
            this.BasicFields = new List<FmsCutterSettingItemDto>();
            this.ExtendFields = new List<FmsCutterSettingItemDto>();
        }

        public IEnumerable<FmsCutterSettingItemDto>  BasicFields { get; set; }

        public IEnumerable<FmsCutterSettingItemDto> ExtendFields { get; set; }
    }

    [AutoMap(typeof(FmsCutterSetting))]
    public class FmsCutterSettingItemDto
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Seq { get; set; }

        public EnumFieldType Type { get; set; }

        public bool IsShow { get; set; }

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}