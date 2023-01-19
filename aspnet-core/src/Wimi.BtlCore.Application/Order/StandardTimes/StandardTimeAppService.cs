using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.StandardTimes.Dtos;

namespace Wimi.BtlCore.Order.StandardTimes
{
    public class StandardTimeAppService : BtlCoreAppServiceBase, IStandardTimeAppService
    {
        private readonly IRepository<Process> processRepository;

        private readonly IRepository<Products.Product> productRepository;

        private readonly IRepository<StandardTime> standardTimeRepository;

        public StandardTimeAppService(
            IRepository<StandardTime> standardTimeRepository,
            IRepository<Products.Product> productRepository,
            IRepository<Process> processRepository)
        {
            this.standardTimeRepository = standardTimeRepository;
            this.productRepository = productRepository;
            this.processRepository = processRepository;
        }

        public async Task CreateOrUpdateStandardTime(StandardTimeDto input)
        {
            if (input.Id == 0) await this.CheckProcessWithProduct(input.ProcessId, input.ProductId);

            var standardTime = ObjectMapper.Map<StandardTime>(input);

            await this.standardTimeRepository.InsertOrUpdateAsync(standardTime);
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteStandardTime(EntityDto input)
        {
            await this.standardTimeRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        ///     获取标准用时信息，可以过滤
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<StandardTimeDto>> GetStandardTime(StandardTimeFilterDto input)
        {
            var standardtimeQuery = this.standardTimeRepository.GetAll();
            var productQuery = this.productRepository.GetAll();
            var processQuery = this.processRepository.GetAll();
            var query = from str in standardtimeQuery
                        join ptr in productQuery on str.ProductId equals ptr.Id
                        join psr in processQuery on str.ProcessId equals psr.Id
                        select new StandardTimeDto
                        {
                            Id = str.Id,
                            ProductId = ptr.Id,
                            ProductCode = ptr.Code,
                            ProductName = ptr.Name,
                            ProcessId = psr.Id,
                            ProcessCode = psr.Code,
                            ProcessName = psr.Name,
                            StandardCostTime = str.StandardCostTime,
                            CycleRate = str.CycleRate,
                            Memo = str.Memo
                        };

            var queryFilter = query.WhereIf(
                !input.Search.Value.IsNullOrWhiteSpace(),
                q => q.ProductCode.Contains(input.Search.Value) || q.ProductName.Contains(input.Search.Value)
                     || q.ProcessCode.Contains(input.Search.Value) || q.ProcessName.Contains(input.Search.Value));
            var result = await queryFilter.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var resultCount = await queryFilter.CountAsync();

            return new DatatablesPagedResultOutput<StandardTimeDto>(
                       resultCount,
                       result,
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        /// <summary>
        ///     每个产品对应工序的标准用时唯一
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private async Task CheckProcessWithProduct(int processId, int productId)
        {
            var query = this.standardTimeRepository.GetAll();

            if (await query.AnyAsync(p => p.ProcessId == processId && p.ProductId == productId))
                throw new UserFriendlyException(this.L("StandardTimeUseRecordAlreadyExists"));
        }
    }
}
