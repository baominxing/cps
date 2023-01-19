using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.Crafts.Dtos;
using Wimi.BtlCore.Order.Processes;

namespace Wimi.BtlCore.Order.Crafts
{
    public class CraftAppService : BtlCoreAppServiceBase, ICraftAppService
    {
        private readonly IRepository<Craft> craftRepository;
        private readonly IRepository<CraftProcess> craftProcessRepository;
        private readonly IRepository<Process> processRepository;
        private readonly ICraftManager craftManager;

        public CraftAppService(
        IRepository<Craft> craftRepository,
        IRepository<CraftProcess> craftProcessRepository,
        IRepository<Process> processRepository,
        ICraftManager craftManagerRepository)
        {
            this.craftRepository = craftRepository;
            this.craftProcessRepository = craftProcessRepository;
            this.processRepository = processRepository;
            this.craftManager = craftManagerRepository;
        }

        [HttpPost]
        public async Task<IEnumerable<CraftDto>> GetCrafts()
        {
            var result = await this.craftRepository.GetAll().ToListAsync();

            return ObjectMapper.Map<IEnumerable<CraftDto>>(result);
        }

        [HttpPost]
        public async Task<CraftDto> GetCraftForEdit(CraftRequestDto input)
        {
            var result = await this.craftRepository.GetAsync(input.Id);

            return ObjectMapper.Map<CraftDto>(result);
        }

        public async Task CreateCraft(CraftRequestDto input)
        {
            await this.CheckCodeName(input);

            var entity = ObjectMapper.Map<Craft>(input);

            await this.craftRepository.InsertAsync(entity);
        }

        public async Task UpdateCraft(CraftRequestDto input)
        {
            var entity = await this.craftRepository.GetAsync(input.Id);

            await this.CheckCodeName(input);

            ObjectMapper.Map(input,entity);

            await this.craftRepository.UpdateAsync(entity);
        }

        public async Task DeleteCraft(EntityDto input)
        {
            await this.craftRepository.DeleteAsync(s => s.Id == input.Id);

            // 删除关联工序组合
            await this.craftProcessRepository.DeleteAsync(s => s.CraftId == input.Id);
        }

        [HttpPost]
        public async Task<PagedResultDto<CraftProcessDto>> GetCraftProcesses(CraftRequestDto input)
        {
            var query = this.craftProcessRepository.GetAll().Where(s => s.CraftId == input.Id);

            var totalCount = await query.CountAsync();

            var result = await query.OrderBy(s => s.ProcessOrder).AsNoTracking().PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<CraftProcessDto>(totalCount, ObjectMapper.Map<List<CraftProcessDto>>(result));
        }

        public async Task CreateCraftProcess(CraftRequestDto input)
        {
            if (!input.ProcessIdList.Any())
            {
                return;
            }

            var craft = await this.craftRepository.GetAll().Include(c=>c.CraftProcesses).FirstOrDefaultAsync(i=> i.Id == input.Id);

            var processes =
                await this.processRepository.GetAll().OrderBy(s => s.Code).Where(s => input.ProcessIdList.Contains(s.Id)).ToListAsync();

            craft.AddCraftProcessList(processes.Select(process => new CraftProcess()
            {
                CraftId = input.Id,
                ProcessId = process.Id,
                ProcessCode = process.Code,
                ProcessName = process.Name
            }));

            craft.ResetProcessOrder();

            await this.craftRepository.UpdateAsync(craft);
            /*
            // 工序索引
            var processOrder = 1;

            // 更新原有最后一道工序的[最后工序]字段
            var originalLastCraftProccess =
                await
                    this.craftProcessRepository.GetAll()
                        .Where(s => s.CraftId == input.Id)
                        .OrderByDescending(s => s.ProcessOrder)
                        .FirstOrDefaultAsync();



            if (originalLastCraftProccess != null)
            {
                originalLastCraftProccess.IsLastProcess = false;
                processOrder = originalLastCraftProccess.ProcessOrder + 1;
            }
            
            // 先取得需要的工序集合,避免在循环中获取
            var processes =
                await this.processRepository.GetAll().OrderBy(s => s.Code).Where(s => input.ProcessIdList.Contains(s.Id)).ToListAsync();

            for (int i = 0; i < input.ProcessIdList.Count; i++)
            {
                var process = processes.First(s => s.Id == input.ProcessIdList[i]);
                if (process == null)
                {
                    continue;
                }

                var entity = new CraftProcess()
                {
                    CraftId = input.Id,
                    ProcessId = process.Id,
                    ProcessCode = process.Code,
                    ProcessName = process.Name,
                    ProcessOrder = processOrder,
                    IsLastProcess = i == (input.ProcessIdList.Count - 1)
                };

                await this.craftProcessRepository.InsertAsync(entity);

                processOrder++;
            }*/
        }

        /// <summary>
        /// 更新工艺工序的排序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateCraftProcess(List<CraftProcessRequestDto> input)
        {
            if (!input.Any())
            {
                return;
            }

            var craftId = input[0].CraftId;

            var craft = await this.craftRepository.GetAllIncluding(x=>x.CraftProcesses).FirstAsync(x=>x.Id == craftId);

            var idIncrementDict = input.ToDictionary(c => c.Id, c => c.Increment);
            craft.UpdateProcessListOrder(idIncrementDict);

            craft.ResetProcessOrder();

            await this.craftRepository.UpdateAsync(craft);

            /*
            var totalPrcoessCount = await this.craftProcessRepository.CountAsync(s => s.CraftId == craftId);

            foreach (var item in input)
            {
                var process = await this.craftProcessRepository.GetAsync(item.Id);
                if (process == null)
                {
                    continue;
                }

                process.ProcessOrder += item.Increment;
                process.IsLastProcess = process.ProcessOrder == totalPrcoessCount;

                await this.craftProcessRepository.UpdateAsync(process);
            }*/
        }

        /// <summary>
        /// 删除工艺工序
        /// 删除后,需要调整工艺工序顺序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteCraftProcess(CraftProcessRequestDto input)
        {
            await this.craftProcessRepository.DeleteAsync(s => s.Id == input.Id);

            await this.CurrentUnitOfWork.SaveChangesAsync();

            var craft = await this.craftRepository.GetAllIncluding(x => x.CraftProcesses).FirstAsync(x => x.Id == input.CraftId);

            craft.ResetProcessOrderAfterDelete(input.Id);

            await this.craftRepository.UpdateAsync(craft);
        }

        public async Task<bool> CraftIsInProcess(EntityDto input)
        {
            return await this.craftManager.CraftIsInProcess(input.Id);
        }

        private async Task CheckCodeName(CraftRequestDto input)
        {
            await this.craftManager.CraftCodeIsUnique(input.Id, input.Code);
            await this.craftManager.CraftNameIsUnique(input.Id, input.Name);
        }
    }
}
