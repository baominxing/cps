using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface IPlanComponentManager : IDomainService
    {
        Task<ApiResponseObject> ListPlanRate();
    }
}