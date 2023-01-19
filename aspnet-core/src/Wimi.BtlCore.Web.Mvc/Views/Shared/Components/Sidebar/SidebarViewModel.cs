using Abp.Application.Navigation;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Sidebar
{
    public class SidebarViewModel
    {

        public string CurrentPageName { get; set; }

        public UserMenu Menu { get; set; }
    }
}
