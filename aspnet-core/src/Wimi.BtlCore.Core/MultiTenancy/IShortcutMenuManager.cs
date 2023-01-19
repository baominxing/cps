using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Domain.Services;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.MultiTenancy
{
    public interface IShortcutMenuManager : IDomainService
    {
        Task BindMenus(long userId, IEnumerable<Menu> menus);

        Task ClearAll(long userId);

        Task<IEnumerable<Menu>> ListBindMenus(long userId);

        Task RestoreDefaults(long userId);

        Task UnBindMenu(long userId, string menuName);
    }
}
