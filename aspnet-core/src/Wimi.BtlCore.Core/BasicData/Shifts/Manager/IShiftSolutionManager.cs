using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Services;

namespace Wimi.BtlCore.BasicData.Shifts
{
    public interface IShiftSolutionManager : IDomainService
    {
        Task<IEnumerable<NameValueDto<IEnumerable<ShiftSolutionItem>>>> ListShiftSolution();

        void CheckIsInUsing(int shiftSolutionId);

        Task DeleteById(int shiftSolutionId);
    }
}
