using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    using System;
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;
    using Abp.Domain.Services;
    using Abp.Domain.Uow;
    using Wimi.BtlCore.Order.Crafts;
    using Wimi.BtlCore.Order.Products;

    public class ProductionPlanManager : BtlCoreDomainServiceBase, IProductionPlanManager
    {
        private readonly ICraftManager craftManager;

        private readonly IRepository<ProductionPlan> productionPlanRepository;

        private readonly IProductManager productManager;

        public ProductionPlanManager(
            IProductManager productManager,
            ICraftManager craftManager,
            IRepository<ProductionPlan> productionPlanRepository)
        {
            this.productManager = productManager;
            this.craftManager = craftManager;
            this.productionPlanRepository = productionPlanRepository;
        }

        public async Task<bool> CanCreate()
        {
            var productIsExist = await this.productManager.IsExist();

            if (!productIsExist)
            {
                throw new ProductNotExistException();
            }

            var craftIsExist = await this.craftManager.IsExist();
            if (!craftIsExist)
            {
                throw new CarftNotExistException();
            }

            return true;
        }

        public async Task<bool> CanDelete(int id)
        {
            var plan = await this.productionPlanRepository.GetAsync(id);
            return plan.IsPrepared();
        }

        public async Task<string> GenerateCode()
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var date = DateTime.Now.Date;
                var codeStart = date.ToString("yyyyMMdd");

                var codeIndex = await this.productionPlanRepository.CountAsync(c => c.Code.StartsWith(codeStart));
                codeIndex = codeIndex + 1;

                var code = $"{codeStart}{codeIndex.ToString("0000")}";
                return code;
            }
        }

        public async Task<bool> IsExist(string code)
        {
            var count = await this.productionPlanRepository.CountAsync(c => c.Code == code);
            return count > 0;
        }
    }
}
