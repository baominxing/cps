using System.Threading.Tasks;

using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    public interface IProductionPlanManager : IDomainService
    {
        Task<bool> CanCreate();

        Task<bool> CanDelete(int id);

        Task<string> GenerateCode();

        Task<bool> IsExist(string code);
    }
}
