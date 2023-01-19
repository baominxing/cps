using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Extensions;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.ProductionPlans;

namespace Wimi.BtlCore.Order.Crafts
{
    public class CraftManager : BtlCoreDomainServiceBase, ICraftManager
    {
        private readonly IRepository<Craft> carftRepository;

        private readonly IRepository<ProductionPlan> productionPlanRepository;

        public CraftManager(IRepository<Craft> carftRepository, IRepository<ProductionPlan> productionPlanRepository)
        {
            this.carftRepository = carftRepository;
            this.productionPlanRepository = productionPlanRepository;
        }

        public async Task CraftCodeIsUnique(int craftId, string code)
        {
            // 如果id不为0，则需要取非自身的code 不存在
            // 如果id==0,则需要判断是否已存在code
            var codeIsExist =
                await
                Task.FromResult(
                    this.carftRepository.GetAll().WhereIf(craftId != 0, c => c.Id != craftId).Any(s => s.Code == code));

            if (codeIsExist)
            {
                throw new UserFriendlyException(this.L("CraftCodeMustBeUnique"));
            }
        }

        public async Task<bool> CraftIsInProcess(int craftId)
        {
            return await Task.FromResult(this.productionPlanRepository.GetAll().Any(s => s.CraftId == craftId));
        }

        public async Task CraftNameIsUnique(int craftId, string code)
        {
            var nameIsExist =
                await
                Task.FromResult(
                    this.carftRepository.GetAll().WhereIf(craftId != 0, c => c.Id != craftId).Any(s => s.Name == code));

            if (nameIsExist)
            {
                throw new UserFriendlyException(this.L("CraftNameMustBeUnique"));
            }
        }

        public async Task<bool> IsExist()
        {
            return await Task.FromResult(this.carftRepository.GetAll().Any());
        }

        public async Task<IEnumerable<NameValue<int>>> ListCarftNameValue()
        {
            var list = await Task.FromResult(this.carftRepository.GetAll().ToList());

            return list.Select(c => new NameValue<int>()
            {
                Name = c.Name,
                Value = c.Id
            });
        }
    }
}
