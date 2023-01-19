using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Crafts.Dtos
{
    [AutoMap(typeof(Craft))]
    public class CraftDto : CraftRequestDto
    {
    }
}
