using Abp.Application.Services.Dto;
using System.Collections.Generic;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Web.Models.Maintain.MaintainOrder
{
    public class MaintainOrderViewModel
    {
        public bool IsEditMode { get; set; } = true;

        public MaintainOrderDto MaintainOrderDto { get; set; } = new MaintainOrderDto();

        public List<ComboboxItemDto> ValidStatusList { get; set; } = new List<ComboboxItemDto>();
    }
}
