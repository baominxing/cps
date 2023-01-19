using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.Products.Dtos;
using Wimi.BtlCore.Plan;

namespace Wimi.BtlCore.Order.Product
{
    public class ProductAppService : BtlCoreAppServiceBase, IProductAppService
    {
        private readonly IRepository<ProductGroup> productGroupRepository;

        private readonly IProductManager productManager;

        private readonly IRepository<Products.Product> productRepository;

        private readonly IRepository<ProcessPlan> planRepository;

        public ProductAppService(
            IRepository<ProductGroup> productGroupRepository,
            IRepository<Products.Product> productRepository,
            IProductManager productManager,
            IRepository<ProcessPlan> planRepository)
        {
            this.productGroupRepository = productGroupRepository;
            this.productRepository = productRepository;
            this.planRepository = planRepository;

            this.productManager = productManager;
        }

        public async Task CreateProduct(ProductRequestDto input)
        {
            await this.CheckCodeName(input);
            var entity = ObjectMapper.Map<Products.Product>(input); 
            await this.productRepository.InsertAsync(entity);
        }

        public async Task CreateProductGroup(ProductGroupRequestDto input)
        {
            await this.CheckCodeName(input);

            var entity = ObjectMapper.Map<ProductGroup>(input);  
            await this.productGroupRepository.InsertAsync(entity);
        }

        public async Task<bool> ProductIsInProgress(ProductRequestDto input)
        {
            var inProgress = await this.planRepository.GetAll().Where(p => p.ProductId == input.Id && p.Status == EnumPlanStatus.InProgress).ToListAsync();
            if (inProgress.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task DeleteProduct(ProductRequestDto input)
        {
            await this.productManager.ProductIsInProcessPlan(input.Id);

            await this.productRepository.DeleteAsync(input.Id);

            var planId = await this.planRepository.GetAll().Where(p => p.ProductId == input.Id).Select(p => p.Id).FirstOrDefaultAsync();
            if (planId != 0)
            {
                var plan = await this.planRepository.GetAsync(planId);
                plan.Status = EnumPlanStatus.Complete;
                await this.planRepository.UpdateAsync(plan);
            }

            await this.planRepository.DeleteAsync(p => p.ProductId == input.Id);
        }

        public async Task DeleteProductGroup(ProductGroupRequestDto input)
        {
            //await this.productManager.ProductIsInProcessWithGroupId(input.Id);
            await this.productManager.ProductIsInProcessPlanWithGroupId(input.Id);

            await this.productGroupRepository.DeleteAsync(input.Id);

            // 删除关联的产品
            await this.productRepository.DeleteAsync(s => s.ProductGroupId == input.Id);

            var products = await this.productRepository.GetAll().Where(p => p.ProductGroupId == input.Id).ToListAsync();
            foreach (var product in products)
            {
                var planId = await this.planRepository.GetAll().Where(p => p.ProductId == input.Id).Select(p => p.Id).FirstOrDefaultAsync();
                if (planId != 0)
                {
                    var plan = await this.planRepository.GetAsync(planId);
                    plan.Status = EnumPlanStatus.Complete;
                    await this.planRepository.UpdateAsync(plan);
                }

                await this.planRepository.DeleteAsync(p => p.ProductId == product.Id);
            }

        }

        public async Task<ProductDto> GetProductForEdit(ProductRequestDto input)
        {
            var result = await this.productRepository.GetAsync(input.Id);

            return ObjectMapper.Map<ProductDto>(result);
        }

        public async Task<ProductGroupDto> GetProductGroupForEdit(ProductGroupRequestDto input)
        {
            var result = await this.productGroupRepository.GetAsync(input.Id);

            return ObjectMapper.Map<ProductGroupDto>(result);
        }

        public async Task<IEnumerable<ProductGroupDto>> GetProductGroups()
        {
            var result = await this.productGroupRepository.GetAll().ToListAsync();

            return ObjectMapper.Map<IEnumerable<ProductGroupDto>>(result);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(ProductRequestDto input)
        {

            var result = await this.productRepository.GetAll()
                .Where(s => s.ProductGroupId == input.ProductGroupId)
                .ToListAsync();

            return ObjectMapper.Map<IEnumerable<ProductDto>>(result); 
        }

        public IEnumerable<ProductDto> ListProducts()
        {
            var result = this.productRepository.GetAll()
                .ToList();

            return ObjectMapper.Map<IEnumerable<ProductDto>>(result); 
        }

        public async Task UpdateProduct(ProductRequestDto input)
        {
            var entity = await this.productRepository.GetAsync(input.Id);

            await this.CheckCodeName(input);

            ObjectMapper.Map(input, entity);

            await this.productRepository.UpdateAsync(entity);
        }

        public async Task UpdateProductGroup(ProductGroupRequestDto input)
        {
            await this.CheckCodeName(input);

            var entity = await this.productGroupRepository.GetAsync(input.Id);

            ObjectMapper.Map(input, entity);
            await this.productGroupRepository.UpdateAsync(entity);
        }

        private async Task CheckCodeName(ProductGroupRequestDto input)
        {
            await this.productManager.ProductGroupCodeIsUnique(input.Id, input.Code);
            await this.productManager.ProductGroupNameIsUnique(input.Id, input.Name);
        }

        private async Task CheckCodeName(ProductRequestDto input)
        {
            await this.productManager.ProductCodeIsUnique(input.Id, input.Code);
            await this.productManager.ProductNameIsUnique(input.Id, input.Name);
        }

        public async Task<ListResultDto<GetProductsTreeDto>> GetProductsTree()
        {
            var listGroups = await productGroupRepository.GetAllIncluding(x => x.Products).ToListAsync();

            return new ListResultDto<GetProductsTreeDto>(
                listGroups.Select(
                        x =>
                        {
                            var dto = new GetProductsTreeDto();
                            dto.GroupName = x.Name;
                            dto.Children.AddRange(
                                x.Products.Select(s => new GetProductChildrenTreeDto()
                                {
                                    Id = s.Id,
                                    Name = s.Name
                                })
                            );
                            return dto;
                        })
                    .ToList());
        }
    }
}
