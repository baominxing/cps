namespace Wimi.BtlCore.MultiTenancy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Configuration;

    public class ShortcutMenuManager : BtlCoreDomainServiceBase, IShortcutMenuManager
    {
        private readonly IRepository<ShortcutMenu> shourcutMenuRepository;

        public ShortcutMenuManager(IRepository<ShortcutMenu> shourcutMenuRepository)
        {
            this.shourcutMenuRepository = shourcutMenuRepository;
        }

        public async Task BindMenus(long userId, IEnumerable<Menu> menus)
        {
            await this.ClearAll(userId);

            IList<ShortcutMenu> shourtcutMenus = new List<ShortcutMenu>();

            foreach (var menu in menus)
            {
                var shortcutMenu = new ShortcutMenu();
                shortcutMenu.BindUserMenu(userId, menu);
                shourtcutMenus.Add(shortcutMenu);
            }

            foreach (var shourtcutMenu in shourtcutMenus)
            {
                await this.shourcutMenuRepository.InsertAsync(shourtcutMenu);
            }
        }

        public async Task ClearAll(long userId)
        {
            await this.shourcutMenuRepository.DeleteAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Menu>> ListBindMenus(long userId)
        {
            var list = await this.shourcutMenuRepository.GetAllListAsync(c => c.UserId == userId);

            return list.Select(c => c.Menu).AsEnumerable();
        }

        public async Task RestoreDefaults(long userId)
        {
            var menus = new[] { AppSettingNames.DevicesMonitoringStates, AppSettingNames.DevicesRealtimeAlarms, AppSettingNames.VisualView };

            await this.BindMenus(userId, menus);
        }

        public async Task UnBindMenu(long userId, string menuName)
        {
            await this.shourcutMenuRepository.DeleteAsync(c => c.UserId == userId && c.Menu.Name == menuName);
        }
    }
}
