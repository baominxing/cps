using System.Collections.Generic;
using Wimi.BtlCore.CraftMaintain.Dtos;
using Wimi.BtlCore.CustomFields.Dto;

namespace Wimi.BtlCore.Web.Models.CraftMaintain
{
    public class FmsCutterViewModel
    {
        public FmsCutterViewModel()
        {
            Dto = new FmsCutterDto();
            this.Field = new List<CustomFieldDto>();
            this.IsEditMode = false;
        }

        public FmsCutterDto Dto { get; set; }

        public bool IsEditMode { get; set; }

        public IEnumerable<CustomFieldDto> Field { get; set; }
    }
}