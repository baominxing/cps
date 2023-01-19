using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Wimi.BtlCore.Order.ProductionPlans;
using Wimi.BtlCore.Plan;

namespace Wimi.BtlCore.Order.Products
{
    public class ProductManager : BtlCoreDomainServiceBase, IProductManager
    {
        private readonly IRepository<ProductGroup> productGroupRepository;

        private readonly IRepository<ProductionPlan> productionPlanRepository;

        private readonly IRepository<Product> productRepository;

        private readonly IRepository<ProcessPlan> processPlanRepository;

        public ProductManager(
            IRepository<Product> productRepository,
            IRepository<ProductionPlan> productionPlanRepository,
            IRepository<ProductGroup> productGroupRepository,
            IRepository<ProcessPlan> processPlanRepository)
        {
            this.productRepository = productRepository;
            this.productGroupRepository = productGroupRepository;
            this.productionPlanRepository = productionPlanRepository;
            this.processPlanRepository = processPlanRepository;
        }

        public async Task<bool> IsExist()
        {
            return await Task.FromResult(this.productRepository.GetAll().Any());
        }

        public async Task<IEnumerable<ProductNameValue>> ListProductNameValue()
        {
            var list = await Task.FromResult(this.productRepository.GetAll().ToList());

            return list.Select(c => c.ToProductNameValue());
        }

        public async Task ProductCodeIsUnique(int productId, string code)
        {
            // 如果id不为0，则需要取非自身的code 不存在
            // 如果id==0,则需要判断是否已存在code
            var codeIsExist =
                await
                Task.FromResult(
                    this.productRepository.GetAll()
                    .WhereIf(productId != 0, s => s.Id != productId)
                    .Any(s => s.Code == code));

            if (codeIsExist)
            {
                throw new UserFriendlyException(this.L("ProductCodeMustBeUnique"));
            }
        }

        public async Task ProductGroupCodeIsUnique(int productGroupId, string code)
        {
            // 如果id不为0，则需要取非自身的code 不存在
            // 如果id==0,则需要判断是否已存在code
            var codeIsExist =
                await
                Task.FromResult(
                    this.productGroupRepository.GetAll()
                    .WhereIf(productGroupId != 0, c => c.Id != productGroupId)
                    .Any(s => s.Code == code));

            if (codeIsExist)
            {
                throw new UserFriendlyException(this.L("ProductGroupCodeMustBeUnique"));
            }
        }

        public async Task ProductGroupNameIsUnique(int productGroupId, string name)
        {
            var nameIsExist =
                await
                Task.FromResult(
                    this.productGroupRepository.GetAll()
                    .WhereIf(productGroupId != 0, c => c.Id != productGroupId)
                    .Any(s => s.Name == name));
            if (nameIsExist)
            {
                throw new UserFriendlyException(this.L("ProductGroupNameMustBeUnique"));
            }
        }

        public async Task<bool> ProductIsInProcess(int[] productIds)
        {
            return
                await Task.FromResult(this.productionPlanRepository.GetAll().Any(s => productIds.Contains(s.ProductId)));
        }

        public async Task ProductIsInProcess(int productId)
        {
            if (await this.ProductIsInProcess(new[] { productId }))
            {
                throw new UserFriendlyException(this.L("ProductHasBeenPutIntoProduction"));
            }
        }

        public async Task<bool> ProductIsInProcessPlan(int[] productIds)
        {
            return
                await Task.FromResult(this.processPlanRepository.GetAll().Any(s => productIds.Contains(s.ProductId)));
        }

        public async Task ProductIsInProcessPlan(int productId)
        {
            if (await this.ProductIsInProcessPlan(new[] { productId }))
            {
                throw new UserFriendlyException(this.L("ProductHasBeenInPlan"));
            }
        }

        public async Task ProductIsInProcessPlanWithGroupId(int groupId)
        {
            var productIdsOfGroup =
                this.productRepository.GetAll().Where(s => s.ProductGroupId == groupId).Select(s => s.Id).ToList();

            if (await this.ProductIsInProcessPlan(productIdsOfGroup.ToArray()))
            {
                throw new UserFriendlyException(this.L("ExistingProductsUnderTheProductGroup"));
            }
        }

        public async Task ProductIsInProcessWithGroupId(int groupId)
        {
            var productIdsOfGroup =
                this.productRepository.GetAll().Where(s => s.ProductGroupId == groupId).Select(s => s.Id).ToList();

            if (await this.ProductIsInProcess(productIdsOfGroup.ToArray()))
            {
                throw new UserFriendlyException(this.L("ExistingProductsBeginToProduce"));
            }
        }

        public async Task ProductNameIsUnique(int productId, string name)
        {
            var nameIsExist =
                await
                Task.FromResult(
                    this.productRepository.GetAll()
                    .WhereIf(productId != 0, s => s.Id != productId)
                    .Any(s => s.Name == name));

            if (nameIsExist)
            {
                throw new UserFriendlyException(this.L("ProductNameMustBeUnique"));
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            var entity = await this.productRepository.FirstOrDefaultAsync(q => q.Id == id);
            if (entity == null)
            {
                throw new UserFriendlyException(this.L("ProductDoesNotExist"));
            }

            return entity;
        }
    }
}
