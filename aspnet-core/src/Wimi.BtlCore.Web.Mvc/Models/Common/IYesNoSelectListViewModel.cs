using Microsoft.AspNetCore.Mvc.Rendering;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IYesNoSelectListViewModel
    {
        SelectList YesNoModel { get; set; }
    }
}
