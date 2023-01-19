namespace Wimi.BtlCore.ShiftTargetYiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using Microsoft.EntityFrameworkCore;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Order.Products;
    using Wimi.BtlCore.ShiftTargetYiled.Dto;
    using Microsoft.AspNetCore.Mvc;

    [AbpAuthorize(PermissionNames.Pages_BasicData_ShiftTargetYiled)]
    public class ShiftTargetYiledAppService : BtlCoreAppServiceBase, IShiftTargetYiledAppService
    {
        private readonly IRepository<Product> productRepository;

        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;

        private readonly IRepository<ShiftSolution> shiftSolutionRepository;

        private readonly IRepository<ShiftTargetYileds> shiftTargetTiledRepository;

        private readonly IShiftTargetYiledManager shiftTargetYiledManager;

        public ShiftTargetYiledAppService(
            IRepository<ShiftTargetYileds> shiftTargetTiledRepository,
            IShiftTargetYiledManager shiftTargetYiledManager,
            IRepository<Product> productRepository,
            IRepository<ShiftSolution> shiftSolutionRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository)
        {
            this.shiftTargetTiledRepository = shiftTargetTiledRepository;
            this.shiftTargetYiledManager = shiftTargetYiledManager;
            this.productRepository = productRepository;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
        }

        public async Task Create(ShiftTargetYiledDto input)
        {
            try
            {
                var checkResult = this.shiftTargetYiledManager.CheckShiftDay(
                    input.StartTime,
                    input.EndTime,
                    input.ProductId,
                    input.ShiftSolutionItemId);
                if (string.IsNullOrEmpty(checkResult))
                {
                    var targetYiled = ObjectMapper.Map<ShiftTargetYileds>(input);
                    var shiftList = this.shiftTargetYiledManager.ShiftDayList(input.StartTime, input.EndTime);
                    foreach (var item in shiftList)
                    {
                        targetYiled.ShiftDay = item;
                        await this.shiftTargetTiledRepository.InsertAsync(targetYiled);
                        await this.CurrentUnitOfWork.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new UserFriendlyException(this.L("ProductHasBeenRecorded{0}", checkResult));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task Delete(EntityDto input)
        {
            await this.shiftTargetTiledRepository.DeleteAsync(input.Id);
        }

        [HttpPost]
        public async Task<IEnumerable<NameValueDto<int>>> GetProductForEdit()
        {
            return await this.productRepository.GetAll()
                       .Select(p => new NameValueDto<int> { Name = p.Name, Value = p.Id })
                       .ToListAsync();
        }

        [HttpPost]
        public async Task<IEnumerable<NameValueDto<int>>> GetShiftSolutionForEdit()
        {
            return await this.shiftSolutionRepository.GetAll()
                       .Select(s => new NameValueDto<int> { Name = s.Name, Value = s.Id })
                       .ToListAsync();
        }

        [HttpPost]
        public async Task<IEnumerable<NameValueDto<int>>> GetShiftSolutionItemForEdit(EntityDto input)
        {
            return await this.shiftSolutionItemRepository.GetAll()
                       .Where(s => s.ShiftSolutionId == input.Id)
                       .Select(s => new NameValueDto<int> { Name = s.Name, Value = s.Id })
                       .ToListAsync();
        }

        public async Task<DatatablesPagedResultOutput<ShiftTargetYiledDto>> ShiftTargetYiledList(
            ShiftTargetYiledRequestDto input)
        {
            var query = this.shiftTargetTiledRepository.GetAll()
                .Select(
                    s => new ShiftTargetYiledDto
                             {
                                 Id = s.Id,
                                 ShiftDay = s.ShiftDay,
                                 ShiftSolutionItemId = s.ShiftSolutionItemId,
                                 ShiftName = s.ShiftSolutionItem.Name,
                                 ProductId = s.ProductId,
                                 ProductCode = s.Products.Code,
                                 ProductName = s.Products.Name,
                                 ProductSpec = s.Products.Spec,
                                 TargetYiled = s.TargetYiled
                             })
                .WhereIf(
                    !string.IsNullOrEmpty(input.ProductInput),
                    q => q.ProductCode.ToLower().Contains(input.ProductInput.ToLower())
                         || q.ProductName.ToLower().Contains(input.ProductInput.ToLower()))
                .WhereIf(
                    !string.IsNullOrEmpty(input.ShiftName),
                    q => q.ShiftName.ToLower().Contains(input.ShiftName.Trim().ToLower()))
                .WhereIf(
                    input.StartTime.HasValue && input.EndTime.HasValue,
                    q => q.ShiftDay >= input.StartTime.Value && q.ShiftDay <= input.EndTime.Value);

            var count = await query.CountAsync();
            var result = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new DatatablesPagedResultOutput<ShiftTargetYiledDto>(count, result, count, input.Draw);
        }

        public async Task Update(ShiftTargetYiledDto input)
        {
            if (!input.Id.HasValue) return;

            var targetYiled = await this.shiftTargetTiledRepository.GetAsync(input.Id.Value);
            targetYiled.TargetYiled = input.TargetYiled;
        }
    }
}