namespace Wimi.BtlCore.Web.Models.BasicData.ShiftTargetYiled
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.ShiftTargetYiled;
    using Wimi.BtlCore.ShiftTargetYiled.Dto;

    [AutoMap(typeof(ShiftTargetYileds))]
    public class ShiftTargetYiledViewModel : ShiftTargetYiledDto
    {
    }
}