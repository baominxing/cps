using Abp.AutoMapper;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FmsCutterExtend))]
    public class FmsCutterExtendDto
    {
        public string Code { get; set; }

        public int FmsCutterId { get; set; }

        public int CustomFieldId { get; set; }

        public string FieldValue { get; set; }
    }
}