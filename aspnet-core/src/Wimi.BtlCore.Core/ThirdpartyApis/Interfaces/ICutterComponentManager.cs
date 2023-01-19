using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface ICutterComponentManager:IDomainService
    {
        Task<ApiResponseObject> ListToolWarnings();
    }
}