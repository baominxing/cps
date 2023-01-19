using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.Crafts;
using Wimi.BtlCore.Order.ProductionPlans.Dtos;
using Wimi.BtlCore.Order.Products;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    [AbpAuthorize(PermissionNames.Pages_Order_ProductionPlan_Manage)]
    public class ProductionPlanAppService : BtlCoreAppServiceBase, IProductionPlanAppService
    {
        private readonly ICraftManager carftManager;

        private readonly IRepository<CraftProcess> craftProcessRepository;

        private readonly IProductionPlanManager productionPlanManager;

        private readonly IRepository<ProductionPlan> productionPlanRepository;

        private readonly IProductManager productManager;

        private readonly IAddWorkOrderManager workOrderManager;

        private readonly IRepository<WorkOrders.WorkOrder> workOrderRepository;

        public ProductionPlanAppService(
            IRepository<ProductionPlan> productionPlanRepository,
            IProductionPlanManager productionPlanManager,
            IProductManager productManager,
            IAddWorkOrderManager workOrderManager,
            ICraftManager carftManager,
            IRepository<CraftProcess> craftProcessRepository,
            IRepository<WorkOrders.WorkOrder> workOrderRepository)
        {
            this.productionPlanRepository = productionPlanRepository;
            this.productionPlanManager = productionPlanManager;
            this.productManager = productManager;
            this.workOrderManager = workOrderManager;
            this.carftManager = carftManager;
            this.craftProcessRepository = craftProcessRepository;
            this.workOrderRepository = workOrderRepository;
        }

        public async Task<bool> CanCreate()
        {
            try
            {
                return await this.productionPlanManager.CanCreate();
            }
            catch (ProductNotExistException)
            {
                throw new UserFriendlyException(this.L("ProductNotExist"));
            }
            catch (CarftNotExistException)
            {
                throw new UserFriendlyException(this.L("ProcessDoesNotExist"));
            }
        }

        public async Task<ChangeWorkOrderVolumeDto> ChangeWorkOrderVolume(ChangeWorkOrderVolumeDto input)
        {
            var workdOrder = await this.workOrderRepository.GetAsync(input.WorkOrderId);
            workdOrder.AimVolume = input.AimVolume;
            workdOrder.PutVolume = input.PutVolume;
            await this.workOrderRepository.UpdateAsync(workdOrder);
            return input;
        }

        public async Task CreateOrUpdate(ProductionPlanDto input)
        {
            if (!input.Id.HasValue)
            {
                await this.Create(input);
                return;
            }

            await this.Update(input);
        }

        public async Task Delete(EntityDto input)
        {
            var canDelete = await this.productionPlanManager.CanDelete(input.Id);
            if (!canDelete) throw new UserFriendlyException(this.L("CannotDeleteThisPlan"));

            await this.productionPlanRepository.DeleteAsync(input.Id);
        }

        public async Task<GetProductionPlanForEditDto> GetProductionPlanForEdit(NullableIdDto input)
        {
            var result = new GetProductionPlanForEditDto
            {
                ProductionPlan = new ProductionPlanDto(),
                ProductList = new List<ProductNameValueDto>(),
                CarftList = new List<NameValueDto<int>>()
            };

            var productNameValues = await this.productManager.ListProductNameValue();

            result.ProductList = ObjectMapper.Map<IEnumerable<ProductNameValueDto>>(productNameValues);

            var carfts = await this.carftManager.ListCarftNameValue();
            result.CarftList = ObjectMapper.Map<IEnumerable<NameValueDto<int>>>(carfts);

            if (!input.Id.HasValue)
            {
                result.ProductionPlan.Code = await this.productionPlanManager.GenerateCode();

                return await Task.FromResult(result);
            }

            var plan = await this.productionPlanRepository.GetAsync(input.Id.Value);
            result.ProductionPlan = ObjectMapper.Map<ProductionPlanDto>(plan);
            return result;
        }

        public async Task<DatatablesPagedResultOutput<ProductionPlanListDto>> ListProductionPlan(
            ListProductionPlanDto input)
        {
            var query = this.productionPlanRepository.GetAllIncluding(p => p.Product, p => p.WorkOrders, p => p.Craft)
                .WhereIf(
                    !input.PlanCode.IsNullOrEmpty(),
                    c => c.Code.Contains(input.PlanCode) || c.OrderCode.Contains(input.PlanCode))
                .WhereIf(
                    !input.ProductCode.IsNullOrEmpty(),
                    c => c.Product.Name.Contains(input.ProductCode) || c.Product.Code.Contains(input.ProductCode));

            var totalCount = await query.CountAsync();
            var filteredCount = await query.CountAsync();

            var pagedQuery = query.OrderBy(input.Sorting).PageBy(input);

            var plans = await pagedQuery.ToListAsync();

            var planListDtos = ObjectMapper.Map<List<ProductionPlanListDto>>(plans);
            await this.FillWorkOrders(planListDtos);

            return new DatatablesPagedResultOutput<ProductionPlanListDto>(
                filteredCount,
                planListDtos,
                totalCount,
                input.Draw);
        }

        private async Task Create(ProductionPlanDto input)
        {
            var plan = ObjectMapper.Map<ProductionPlan>(input);

            var isExist = await this.productionPlanManager.IsExist(plan.Code);
            if (isExist) plan.Code = await this.productionPlanManager.GenerateCode();

            plan.Prepare();

            await this.productionPlanRepository.InsertAsync(plan);

            await this.CurrentUnitOfWork.SaveChangesAsync();

            await this.workOrderManager.CreateWorkOrder(plan.CraftId, plan);
        }

        private async Task FillWorkOrders(IEnumerable<ProductionPlanListDto> plans)
        {
            var productionPlanListDtos = plans as ProductionPlanListDto[] ?? plans.ToArray();

            var carftIds = (from plan in productionPlanListDtos select plan.CraftId).Distinct();

            var carftProcessList = await this.craftProcessRepository.GetAll()
                                       .Where(c => carftIds.Contains(c.CraftId))
                                       .ToListAsync();

            foreach (var productionPlanListDto in productionPlanListDtos)
                foreach (var workOrderDto in productionPlanListDto.WorkOrders)
                {
                    var carftProcess = carftProcessList.FirstOrDefault(
                        c => c.CraftId == productionPlanListDto.CraftId && c.ProcessId == workOrderDto.ProcessId);

                    if (carftProcess == null) continue;

                    workOrderDto.ProcessCode = carftProcess.ProcessCode;
                    workOrderDto.ProcessName = carftProcess.ProcessName;
                    workOrderDto.ProcessOrderSeq = carftProcess.ProcessOrder;
                }
        }

        private async Task Update(ProductionPlanDto input)
        {
            if (input.Id == null)
            {
                throw new AbpValidationException(this.L("NoIncomingUpdatePrimaryKey"));
            }

            var plan = await this.productionPlanRepository.GetAsync(input.Id.Value);
            //  plan = ObjectMapper.Map<ProductionPlan>(input);
            plan.ClientName = input.ClientName;
            plan.OrderCode = input.OrderCode;
            plan.StartDate = input.StartDate;
            plan.EndDate = input.EndDate;
            plan.Memo = input.Memo;
            plan.Unit = input.Unit;
            plan.AimVolume = input.AimVolume;
            plan.PutVolume = input.PutVolume;
            foreach (var workOrderse in plan.WorkOrders)
            {
                workOrderse.AimVolume = input.AimVolume;
                workOrderse.PutVolume = input.PutVolume;
            }

            await this.productionPlanRepository.UpdateAsync(plan);
        }
    }
}
