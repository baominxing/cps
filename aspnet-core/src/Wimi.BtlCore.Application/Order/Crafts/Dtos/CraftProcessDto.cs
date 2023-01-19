using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Crafts.Dtos
{
    [AutoMap(typeof(CraftProcess))]
    public class CraftProcessDto : CraftProcessRequestDto
    {
    }
}
