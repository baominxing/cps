namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Runtime.Caching;
    using Abp.Threading;
    using Abp.UI;
    using Microsoft.EntityFrameworkCore;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.Plan.Manager;
    using Wimi.BtlCore.Runtime.Caching;
    using Wimi.BtlCore.Trace.Manager;

    /// <summary>
    ///     Performs domain logic for Machine Units.
    /// </summary>
    public class DeviceGroupManager : BtlCoreDomainServiceBase
    {
        private readonly ICacheManager cacheManager;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository;
        private readonly IRepository<DeviceGroupPermissionSetting, long> deviceGroupPermissionRepository;
        private readonly TraceFlowSettingManager traceFlowSettingManager;
        private readonly ProcessPlanManager processPlanManager;

        public DeviceGroupManager(
            IRepository<DeviceGroup> deviceGroupRepository,
            ICacheManager cacheManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository,
            IRepository<DeviceGroupPermissionSetting, long> deviceGroupPermissionRepository,
            TraceFlowSettingManager traceFlowSettingManager,
            ProcessPlanManager processPlanManager)
        {
            DeviceGroupRepository = deviceGroupRepository;
            this.cacheManager = cacheManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineShiftEffectiveIntervalRepository = machineShiftEffectiveIntervalRepository;
            this.deviceGroupPermissionRepository = deviceGroupPermissionRepository;
            this.traceFlowSettingManager = traceFlowSettingManager;
            this.processPlanManager = processPlanManager;
        }

        private IRepository<DeviceGroup> DeviceGroupRepository { get; }

        [UnitOfWork]
        public virtual async Task CreateAsync(DeviceGroup deviceGroup)
        {
            deviceGroup.Code = await GetNextChildCodeAsync(deviceGroup.ParentId);
            await ValidateDeviceGroupAsync(deviceGroup);
            await DeviceGroupRepository.InsertAsync(deviceGroup);
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(int id)
        {
            var children = await FindChildrenAsync(id, true);

            foreach (var child in children)
            {
                await DeviceGroupRepository.DeleteAsync(child);
            }

            await DeviceGroupRepository.DeleteAsync(id);

            var deviceGroupIds = children.Select(n => n.Id).ToList();
            deviceGroupIds.Add(id);

            await machineDeviceGroupRepository.DeleteAsync(t => deviceGroupIds.Contains(t.DeviceGroupId));
            await deviceGroupPermissionRepository.DeleteAsync(d => deviceGroupIds.Contains(d.DeviceGroupId));
            await traceFlowSettingManager.DeleteByDeviceGroupId(id);
            await processPlanManager.DeletePlansByDeviceGroupId(id);
        }



        public async Task<List<DeviceGroup>> FindChildrenAsync(int? parentId, bool recursive = false)
        {
            if (recursive)
            {
                if (!parentId.HasValue)
                {
                    return await DeviceGroupRepository.GetAllListAsync();
                }

                var code = await GetCodeAsync(parentId.Value);
                return
                    await
                    DeviceGroupRepository.GetAllListAsync(
                        ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value);
            }

            return await DeviceGroupRepository.GetAllListAsync(ou => ou.ParentId == parentId);
        }

        public virtual async Task<List<FlatDeviceGroupDto>> GetCachedDeviceGroups()
        {
            return (await GetDeviceGroupCacheItemAsync()).CatchedDeviceGroups.OrderBy(d => d.Seq).ToList();
        }

        public virtual string GetCode(int id)
        {
            return AsyncHelper.RunSync(() => GetCodeAsync(id));
        }

        public virtual async Task<string> GetCodeAsync(int id)
        {
            return (await DeviceGroupRepository.GetAsync(id)).Code;
        }

        public virtual async Task<DeviceGroup> GetLastChildOrNullAsync(int? parentId)
        {
            var children = await DeviceGroupRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            return children.OrderBy(c => c.Code).LastOrDefault();
        }

        public virtual async Task<string> GetNextChildCodeAsync(int? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return DeviceGroup.AppendCode(parentCode, DeviceGroup.CreateCode(1));
            }

            return DeviceGroup.CalculateNextCode(lastChild.Code);
        }

        [UnitOfWork]
        public virtual async Task MoveAsync(int id, int? parentId)
        {
            var deviceGroup = await DeviceGroupRepository.GetAsync(id);
            if (deviceGroup.ParentId == parentId)
            {
                return;
            }

            // Should find children before Code change
            var children = await FindChildrenAsync(id, true);

            // Store old code of OU
            var oldCode = deviceGroup.Code;

            deviceGroup.Code = await GetNextChildCodeAsync(parentId);
            deviceGroup.ParentId = parentId;

            await ValidateDeviceGroupAsync(deviceGroup);

            // 更新设备与设备组关系的组Code
            var groupRefs = machineDeviceGroupRepository.GetAll().Where(q => q.DeviceGroupId == id);
            foreach (MachineDeviceGroup machineDeviceGroup in groupRefs)
            {
                machineDeviceGroup.DeviceGroupCode = deviceGroup.Code;
            }

            // Update Children Codes
            foreach (var child in children)
            {
                child.Code = DeviceGroup.AppendCode(deviceGroup.Code, DeviceGroup.GetRelativeCode(child.Code, oldCode));

                // 更新子设备与设备组关系中的组Code
                var childGroupRefs = machineDeviceGroupRepository.GetAll().Where(q => q.DeviceGroupId == child.Id);
                foreach (MachineDeviceGroup machineDeviceGroup in childGroupRefs)
                {
                    machineDeviceGroup.DeviceGroupCode = child.Code;
                }
            }
        }

        /// <summary>
        /// 获取当天在班次有效期内的产线
        /// </summary>
        /// <returns></returns>
        public async Task<List<DeviceGroup>> ListDeviceGroupIdsForMachineShiftEffectiveInterval()
        {
            var startTime = DateTime.Today;
            var endTime = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            var result = (await this.machineShiftEffectiveIntervalRepository.GetAll().Where(x => x.StartTime <= startTime && x.EndTime >= endTime)
                .ToListAsync()).Select(x => x.MachineId).Distinct();

            var query = from q in machineDeviceGroupRepository.GetAll().Where(x => result.Contains(x.MachineId))
                        join deviceGroup in DeviceGroupRepository.GetAll().Where(d => !d.ParentId.HasValue) on q.DeviceGroupId equals deviceGroup.Id
                        orderby deviceGroup.Code ascending
                        select deviceGroup;

            var list = await query.ToListAsync();

            return list.GroupBy(x => x.Id, (key, g) => g.First()).ToList();
        }

        public async Task<IEnumerable<DeviceGroup>> ListFirstClassDeviceGroups()
        {
            var result = await this.DeviceGroupRepository.GetAll().Where(d => !d.ParentId.HasValue).OrderBy(d => d.Seq)
                .ToListAsync();
            return result;
        }

        public virtual async Task UpdateAsync(DeviceGroup deviceGroup)
        {
            await ValidateDeviceGroupAsync(deviceGroup);
            await DeviceGroupRepository.UpdateAsync(deviceGroup);
        }

        protected virtual async Task ValidateDeviceGroupAsync(DeviceGroup deviceGroup)
        {
            var siblings =
                (await FindChildrenAsync(deviceGroup.ParentId)).Where(ou => ou.Id != deviceGroup.Id).OrderBy(d => d.Seq).ToList();

            if (siblings.Any(ou => ou.DisplayName == deviceGroup.DisplayName))
            {
                throw new UserFriendlyException(this.L("DeviceGroupAlreadyExist{0}", deviceGroup.DisplayName));
            }
        }

        private async Task<DeviceGroupCacheItem> GetDeviceGroupCacheItemAsync()
        {
            var cacheKey = DeviceGroupCacheItem.CacheStoreName;

            return await cacheManager.GetDeviceGroupCache().GetAsync(cacheKey,
                async () =>
                    {
                        var newCacheItem = new DeviceGroupCacheItem();
                        var groups = await Task.Run(
                            () => (from dg in DeviceGroupRepository.GetAll().AsEnumerable()
                                   join mdg in machineDeviceGroupRepository.GetAll().AsEnumerable() on dg.Id equals mdg.DeviceGroupId into g
                                   select new FlatDeviceGroupDto
                                   {
                                       Code = dg.Code,
                                       DisplayName = dg.DisplayName,
                                       Id = dg.Id,
                                       Seq = dg.Seq,
                                       ParentId = dg.ParentId,
                                       MemberCount = g.Count()
                                   }).ToList());

                        groups.ForEach(item => newCacheItem.CatchedDeviceGroups.Add(item));

                        return newCacheItem;
                    });
        }

        public async Task<DeviceGroup> GetFirstClassDeviceGroupById(int id)
        {
            var deviceGroup = await this.DeviceGroupRepository.FirstOrDefaultAsync(d => d.Id == id);
            if (deviceGroup == null)
            {
                return null;
            }

            if (deviceGroup.ParentId == null)
            {
                return deviceGroup;
            }

            return await this.GetFirstClassDeviceGroupById((int)deviceGroup.ParentId);
        }
    }
}