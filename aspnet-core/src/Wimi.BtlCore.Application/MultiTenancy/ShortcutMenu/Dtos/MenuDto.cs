using System.ComponentModel.DataAnnotations;

using Abp.AutoMapper;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.MultiTenancy.ShortcutMenu.Dtos
{
    [AutoMap(typeof(Menu))]
    public class MenuDto
    {
        [Required]
        public string DisplayName { get; set; }

        public string Icon { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
