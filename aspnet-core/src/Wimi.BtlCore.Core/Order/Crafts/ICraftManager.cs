using System.Collections.Generic;
using System.Threading.Tasks;

using Abp;
using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.Crafts
{
    public interface ICraftManager : IDomainService
    {
        Task CraftCodeIsUnique(int craftId, string code);

        Task<bool> CraftIsInProcess(int craftId);

        Task CraftNameIsUnique(int craftId, string name);

        Task<bool> IsExist();

        Task<IEnumerable<NameValue<int>>> ListCarftNameValue();
    }
}
