using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.Products
{
    public interface IProductManager : IDomainService
    {
        Task<bool> IsExist();

        Task<IEnumerable<ProductNameValue>> ListProductNameValue();

        Task ProductCodeIsUnique(int productGroupId, string code);

        Task ProductGroupCodeIsUnique(int productGroupId, string code);

        Task ProductGroupNameIsUnique(int productGroupId, string name);

        Task<bool> ProductIsInProcess(int[] productIds);

        Task ProductIsInProcess(int productId);

        Task ProductIsInProcessWithGroupId(int groupId);

        Task ProductNameIsUnique(int productGroupId, string name);

        Task<bool> ProductIsInProcessPlan(int[] productIds);

        Task ProductIsInProcessPlan(int productId);

        Task ProductIsInProcessPlanWithGroupId(int groupId);
    }
}
