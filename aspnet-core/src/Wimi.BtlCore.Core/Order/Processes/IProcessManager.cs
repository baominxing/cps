using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Order.Processes
{
    public interface IProcessManager : IDomainService
    {
        Task<bool> CodeIsExist(string code);

        Task<bool> NameIsExist(string name);
    }
}
