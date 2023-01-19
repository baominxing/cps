using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Authorization
{
    [Owned]
    public class Menu
    {
        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
