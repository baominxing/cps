using Abp.AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.BasicData.StateInfo
{
    [AutoMap(typeof(CreateOrUpdateStateInfoDto))]
    public class StateInfoViewModel : CreateOrUpdateStateInfoDto, IYesNoSelectListViewModel
    {
        public StateInfoViewModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }

        public SelectList YesNoModel { get; set; }
    }
}
