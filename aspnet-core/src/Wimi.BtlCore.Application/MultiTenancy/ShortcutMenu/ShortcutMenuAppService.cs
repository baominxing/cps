using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Runtime.Session;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.MultiTenancy.ShortcutMenu.Dtos;

namespace Wimi.BtlCore.MultiTenancy.ShortcutMenu
{
    [AbpAuthorize]
    public class ShortcutMenuAppService : BtlCoreAppServiceBase, IShortcutMenuAppService
    {
        private readonly IShortcutMenuManager shortcutMenuManager;

        public ShortcutMenuAppService(IShortcutMenuManager shortcutMenuManager)
        {
            this.shortcutMenuManager = shortcutMenuManager;
        }

        public async Task BindMenus(IEnumerable<MenuDto> input)
        {
            var userId = this.AbpSession.GetUserId();
            var menus = ObjectMapper.Map<IEnumerable<Menu>>(input);
            await this.shortcutMenuManager.BindMenus(userId, menus);
        }

        public async Task<IEnumerable<MenuDto>> ListBindMenu()
        {
            var userId = this.AbpSession.GetUserId();
            var menus = await this.shortcutMenuManager.ListBindMenus(userId);
            return ObjectMapper.Map<IEnumerable<MenuDto>>(menus);
        }

        public async Task RemoveAll()
        {
            var userId = this.AbpSession.GetUserId();
            await this.shortcutMenuManager.ClearAll(userId);
        }

        public async Task RestoreDefaults()
        {
            var userId = this.AbpSession.GetUserId();
            await this.shortcutMenuManager.RestoreDefaults(userId);
        }

        public async Task UnBindMenu(MenuDto menu)
        {
            var userId = this.AbpSession.GetUserId();
            await this.shortcutMenuManager.UnBindMenu(userId, menu.Name);
        }
    }
}
