using System.Linq;
using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    public interface IAddWorkOrderManager : IDomainService
    {
        Task CreateWorkOrder(int productCraftId, ProductionPlan productPlanId);
    }
}
