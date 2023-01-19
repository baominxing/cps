namespace Wimi.BtlCore.ShiftTargetYiled
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.ShiftTargetYiled.Dto;

    public interface IShiftTargetYiledAppService : IApplicationService
    {
        Task Create(ShiftTargetYiledDto input);

        Task Delete(EntityDto input);

        Task<IEnumerable<NameValueDto<int>>> GetProductForEdit();

        Task<IEnumerable<NameValueDto<int>>> GetShiftSolutionForEdit();

        Task<IEnumerable<NameValueDto<int>>> GetShiftSolutionItemForEdit(EntityDto input);

        Task<DatatablesPagedResultOutput<ShiftTargetYiledDto>> ShiftTargetYiledList(ShiftTargetYiledRequestDto input);

        Task Update(ShiftTargetYiledDto input);
    }
}