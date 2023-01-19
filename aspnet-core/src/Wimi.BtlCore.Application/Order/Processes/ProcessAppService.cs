using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.Processes.Dtos;

namespace Wimi.BtlCore.Order.Processes
{
    [AbpAuthorize(PermissionNames.Pages_Order_Process)]
    public class ProcessAppService : BtlCoreAppServiceBase, IProcessAppService
    {
        private readonly IProcessManager processManager;

        private readonly IRepository<Process> processRepository;

        public ProcessAppService(IRepository<Process> processRepository, IProcessManager processManager)
        {
            this.processRepository = processRepository;
            this.processManager = processManager;
        }

        /// <summary>
        ///     新建或修改工序信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateProcess(ProcessDto input)
        {
            if (input.Id == 0)
            {
                // Create
                await this.CheckCode(input.Code);
                await this.CheckName(input.Name);
                var createEntity = ObjectMapper.Map<Process>(input);
                await this.processRepository.InsertAsync(createEntity);

                return;
            }

            // update
            var entity = await this.processRepository.GetAsync(input.Id);

            if (entity.Name != input.Name)
            {
                await this.CheckName(input.Name);
            }

            ObjectMapper.Map(input,entity);

            await this.processRepository.UpdateAsync(entity);
        }

        /// <summary>
        ///     删除工序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteProcess(EntityDto input)
        {
            await this.processRepository.DeleteAsync(input.Id);
        }

        [HttpPost]
        public async Task<PagedResultDto<ProcessDto>> GetProcess(ProcessFilterDto input)
        {
            var query = this.processRepository.GetAll()
                .WhereIf(
                    !input.Search.Value.IsNullOrWhiteSpace(),
                    g => g.Name.Contains(input.Search.Value) || g.Memo.Contains(input.Search.Value)
                         || g.Code.Contains(input.Search.Value));
            var result = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<ProcessDto>(
                       resultCount,
                        ObjectMapper.Map<List<ProcessDto>>(result),
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        private async Task CheckCode(string code)
        {
            if (await this.processManager.CodeIsExist(code)) throw new UserFriendlyException(this.L("ProcessCodeMustBeUnique"));
        }

        private async Task CheckName(string name)
        {
            if (await this.processManager.NameIsExist(name)) throw new UserFriendlyException(this.L("ProcessNameMustBeUnique"));
        }
    }
}
