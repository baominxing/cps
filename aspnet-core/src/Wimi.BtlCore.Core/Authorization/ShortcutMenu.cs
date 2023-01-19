using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Authorization
{
    [Table("ShortcutMenus")]
    public class ShortcutMenu : Entity
    {
        public ShortcutMenu()
        {
            this.Menu = new Menu();
        }

        public Menu Menu { get; set; }

        public long UserId { get; set; }

        public void BindUserMenu(long userId, Menu menu)
        {
            this.UserId = userId;
            this.Menu = menu;
        }
    }
}
