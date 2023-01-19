using Abp.Application.Services.Dto;
using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.StateInfos;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    public interface IStateInfoManager : IDomainService
    {
        bool IsInUsing(StateInfo input);
        Task<IEnumerable<NameValueDto>> ListFeedbackStates();
    }
}
