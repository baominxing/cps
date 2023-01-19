using Wimi.BtlCore.CustomFields.Dto;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.Web.Models.CraftMaintain
{
    public class FmsCutterSettingViewModel
    {
        public FmsCutterSettingViewModel()
        {
            this.Field = new CustomFieldDto();
        }

        public EnumCustomDisplayType Type { get; set; }

        public bool IsEdit { get; set; }

        public CustomFieldDto Field { get; set; }
    }
}