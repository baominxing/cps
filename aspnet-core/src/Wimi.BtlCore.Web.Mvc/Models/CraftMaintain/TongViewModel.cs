using Wimi.BtlCore.Tongs.Dto;

namespace Wimi.BtlCore.Web.Models.CraftMaintain
{
    public class TongViewModel
    {
        public TongDto Dto { get; set; } = new TongDto();

        public bool IsEditMode { get; set; }
    }
}