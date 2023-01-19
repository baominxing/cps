namespace Wimi.BtlCore.Web.Models.Visual
{
    using System.Collections.Generic;
    using Castle.Components.DictionaryAdapter;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Wimi.BtlCore.Visual.Dto;
    using Wimi.BtlCore.Web.Models.Common;

    public class CreateOrEditNoticesViewModel : GetNoticeInputDto, IYesNoSelectListViewModel
    {
        public CreateOrEditNoticesViewModel()
        {
            this.WorkShopList = new EditableList<GetWorkShopsDto>();
        }

        public bool IsEditModel { get; set; }

        public List<GetWorkShopsDto> WorkShopList { get; set; }

        public SelectList YesNoModel { get; set; }
    }
}