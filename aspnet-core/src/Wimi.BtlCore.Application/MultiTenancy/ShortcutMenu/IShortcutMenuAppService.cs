using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services;
using Wimi.BtlCore.MultiTenancy.ShortcutMenu.Dtos;

namespace Wimi.BtlCore.MultiTenancy.ShortcutMenu
{
    public interface IShortcutMenuAppService : IApplicationService
    {
        Task BindMenus(IEnumerable<MenuDto> input);

        Task<IEnumerable<MenuDto>> ListBindMenu();

        Task RemoveAll();

        Task RestoreDefaults();

        Task UnBindMenu(MenuDto menu);
    }
}
