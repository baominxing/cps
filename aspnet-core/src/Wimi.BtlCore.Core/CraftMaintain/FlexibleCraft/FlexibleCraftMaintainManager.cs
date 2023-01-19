using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.CraftMaintain
{
    public class FlexibleCraftMaintainManager : DomainService
    {
        private readonly IRepository<FlexibleCraft> craftRepository;
        private readonly IRepository<FlexibleCraftProcesse> craftProcesseRepository;
        private readonly IRepository<FlexibleCraftProcesseMap> craftProcesseMapRepository;
        private readonly IRepository<FlexibleCraftProcedureCutterMap> craftProcedureCutterMapRepository;

        public FlexibleCraftMaintainManager(IRepository<FlexibleCraft> craftRepository,
            IRepository<FlexibleCraftProcesse> craftProcesseRepository,
            IRepository<FlexibleCraftProcesseMap> craftProcesseMapRepository,
            IRepository<FlexibleCraftProcedureCutterMap> craftProcedureCutterMapRepository)
        {
            this.craftRepository = craftRepository;
            this.craftProcesseRepository = craftProcesseRepository;
            this.craftProcesseMapRepository = craftProcesseMapRepository;
            this.craftProcedureCutterMapRepository = craftProcedureCutterMapRepository;
        }

        /// <summary>
        /// 获取工艺集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<FlexibleCraft>> GetCrafts(int productId)
        {
            return await craftRepository.GetAllListAsync(x => x.ProductId.Equals(productId));
        }

        public async Task<bool> CheckCraft(FlexibleCraft craft)
        {
            if (!craft.IsTransient())
            {
               return (await craftRepository.FirstOrDefaultAsync(x => 
                   x.ProductId.Equals(craft.ProductId) 
                   && x.Name.Equals(craft.Name)
                   && !x.Id.Equals(craft.Id))) != null;
            }
            else
            { 
               return (await craftRepository.FirstOrDefaultAsync(x => x.ProductId.Equals(craft.ProductId) && x.Name.Equals(craft.Name))) != null;
            }
        }

        public async Task<bool> CheckCraftProcesse(FlexibleCraftProcesse craft)
        {
            if (!craft.IsTransient())
            {
                return (await craftProcesseRepository.FirstOrDefaultAsync(x => 
                     x.Name.Equals(craft.Name)
                    && !x.Id.Equals(craft.Id))) != null;
            }
            else
            {
                return (await craftProcesseRepository.FirstOrDefaultAsync(x => x.Name.Equals(craft.Name))) != null;
            }
        }

        /// <summary>
        /// 获取工艺及工艺下的工序集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlexibleCraft> GetCraft(int id)
        {
            var craft = await craftRepository.GetAsync(id);
            (craft.CraftProcesses as List<FlexibleCraftProcesse>).AddRange(await GetCraftProcessesByCraftId(id));
            return craft;
        }

        /// <summary>
        /// 创建工艺
        /// </summary>
        /// <param name="craft">工艺及工序集合</param>
        /// <param name="craftProcedureCutterMaps">刀具集合</param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task CreatCraft(FlexibleCraft craft, List<FlexibleCraftProcedureCutterMap> craftProcedureCutterMaps)
        {
            var craftId = await craftRepository.InsertAndGetIdAsync(craft);
            craft.CraftProcesses.ToList().ForEach(async f =>
            {
                await craftProcesseMapRepository.InsertAsync(new FlexibleCraftProcesseMap()
                {
                    CraftId = craftId,
                    CraftProcesseId = f.Id
                });
            });

            craftProcedureCutterMaps.ForEach(async f =>
            {
                f.CraftId = craftId;
                await craftProcedureCutterMapRepository.InsertAsync(f);
            });
        }

        [UnitOfWork]
        public async Task EditCraft(FlexibleCraft craft, List<FlexibleCraftProcedureCutterMap> craftProcedureCutterMaps)
        {
            craftRepository.Update(craft.Id, x =>
            {
                x.Name = craft.Name;
                x.Version = craft.Version;
            });
            await craftProcesseMapRepository.DeleteAsync(x => x.CraftId.Equals(craft.Id));

            craft.CraftProcesses.ToList().ForEach(async f =>
            {
                await craftProcesseMapRepository.InsertAsync(new FlexibleCraftProcesseMap()
                {
                    CraftId = craft.Id,
                    CraftProcesseId = f.Id
                });
            });

            await craftProcedureCutterMapRepository.DeleteAsync(x => x.CraftId.Equals(craft.Id));

            craftProcedureCutterMaps.ForEach(async f =>
            {
                f.CraftId = craft.Id;
                await craftProcedureCutterMapRepository.InsertAsync(f);
            });
        }

        public async Task<FlexibleCraftProcesse> CreateOrEditCraftProcesse(FlexibleCraftProcesse processe)
        {
            var id = await craftProcesseRepository.InsertOrUpdateAndGetIdAsync(processe);

            return await craftProcesseRepository.GetAllIncluding(x => x.Tong).FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        /// <summary>
        /// 获取工艺下工序集合
        /// </summary>
        /// <param name="craftId"></param>
        /// <returns></returns>
        public async Task<List<FlexibleCraftProcesse>> GetCraftProcessesByCraftId(int craftId)
        {
            var maps = await craftProcesseMapRepository.GetAllIncluding(x => x.CraftProcesse, x => x.CraftProcesse.Tong).Where(x => x.CraftId.Equals(craftId))
                .ToListAsync();
            return maps.Select(s => s.CraftProcesse).ToList();
        }

        /// <summary>
        /// 获取工艺下刀具集合
        /// </summary>
        /// <param name="craftId"></param>
        /// <returns></returns>
        public async Task<List<FlexibleCraftProcedureCutterMap>> GetCraftProcedureCutterMapsByCraftId(int craftId)
        {
            return await craftProcedureCutterMapRepository.GetAllIncluding(x => x.CraftProcesse, x => x.Cutter).Where(x => x.CraftId.Equals(craftId))
                .ToListAsync();
        }

        public async Task<List<FlexibleCraftProcesse>> GetFlexibleCraftProcessesByIds(List<int> craftProcesseIds)
        {
            return await craftProcesseRepository.GetAllIncluding(x => x.Tong)
                .Where(x => craftProcesseIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<FlexibleCraftProcesse>> GetCraftProcesses(int? craftId, string keyWord)
        {
            return await craftProcesseRepository.GetAllIncluding(x => x.Tong)
                .WhereIf(keyWord != null && !keyWord.IsNullOrWhiteSpace(), u => u.Name.Contains(keyWord))
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
        }

        [UnitOfWork]
        public async Task DeleteCraft(int id)
        {
            await craftProcesseMapRepository.DeleteAsync(x => x.CraftId.Equals(id));
            await craftProcedureCutterMapRepository.DeleteAsync(x => x.CraftId.Equals(id));
            await craftRepository.DeleteAsync(x => x.Id.Equals(id));
        }
    }
}
