using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.EntityFrameworkCore.Seed.Host
{
    public class DefaultShortcutMenuCreator
    {
        private readonly BtlCoreDbContext context;

        public DefaultShortcutMenuCreator(BtlCoreDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            var userIds = this.context.Users.Where(c => c.TenantId.HasValue).Select(c => c.Id);

            IList<ShortcutMenu> menus = new List<ShortcutMenu>();

            foreach (var userId in userIds)
            {
                menus.Add(new ShortcutMenu { UserId = userId, Menu = AppSettingNames.DevicesMonitoringStates });

                menus.Add(new ShortcutMenu { UserId = userId, Menu = AppSettingNames.DevicesRealtimeAlarms });

                menus.Add(new ShortcutMenu { UserId = userId, Menu = AppSettingNames.VisualView });
            }

            foreach (var shortcutMenu in menus)
            {
                var isExist = this.context.ShortcutMenus.Any(
                    c => c.UserId == shortcutMenu.UserId && c.Menu.Name == shortcutMenu.Menu.Name);
                if (!isExist) this.context.ShortcutMenus.Add(shortcutMenu);
            }

            this.context.SaveChanges();
        }
    }
}
