using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Dmps.Manager;
using Wimi.BtlCore.Feedback.Manager;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Runtime.Caching;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.StaffPerformance;
using Wimi.BtlCore.Storage;
using Wimi.BtlCore.Trace.Manager;
using System.Drawing;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    [Audited]
    public class MachineManager : BtlCoreDomainServiceBase, IMachineManager
    {
        private readonly IBtlFolders appFolders;
        private readonly ICacheManager cacheManager;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<BinaryObject, Guid> binaryObjectRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly RoleManager roleManager;
        private readonly IRepository<DeviceGroupYieldMachine> deviceGroupYieldMachineRepository;
        private readonly MachinesShiftDetailsManager machinesShiftDetailsManager;
        private readonly MachineShiftEffectiveIntervalManager machineShiftEffectiveIntervalManager;
        private readonly TraceFlowSettingManager traceFlowSettingManager;
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;
        private readonly PerformanceDeviceManager performanceDeviceManager;
        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly DmpManager dmpManager;
        private readonly IReasonFeedbackManager reasonFeedbackManager;

        public MachineManager(
            IRepository<Machine> machineRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            ICacheManager cacheManager,
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            IBtlFolders appFolders,
            DeviceGroupManager deviceGroupManager,
            RoleManager roleManager, 
            IRepository<DeviceGroupYieldMachine> deviceGroupYieldMachineRepository,
            MachinesShiftDetailsManager machinesShiftDetailsManager,
            MachineShiftEffectiveIntervalManager machineShiftEffectiveIntervalManager,
            TraceFlowSettingManager traceFlowSettingManager,
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository,
            PerformanceDeviceManager performanceDeviceManager,
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            MongoMachineManager mongoMachineManager,
            IUnitOfWorkManager unitOfWorkManager,
            DmpManager dmpManager,
            IReasonFeedbackManager reasonFeedbackManager)
        {
            this.cacheManager = cacheManager;
            this.binaryObjectRepository = binaryObjectRepository;
            this.appFolders = appFolders;
            this.machineRepository = machineRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.AbpSession = NullAbpSession.Instance;
            this.deviceGroupManager = deviceGroupManager;
            this.roleManager = roleManager;
            this.deviceGroupYieldMachineRepository = deviceGroupYieldMachineRepository;
            this.machinesShiftDetailsManager = machinesShiftDetailsManager;
            this.machineShiftEffectiveIntervalManager = machineShiftEffectiveIntervalManager;
            this.traceFlowSettingManager = traceFlowSettingManager;
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
            this.performanceDeviceManager = performanceDeviceManager;
            this.performancePersonnelOnDeviceRepository = performancePersonnelOnDeviceRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.mongoMachineManager = mongoMachineManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.dmpManager = dmpManager;
            this.reasonFeedbackManager = reasonFeedbackManager;
        }

        public IAbpSession AbpSession { get; set; }

        public virtual async Task AddToDeviceGroupAsync(int machineId, int deviceGroupId)
        {
            await
                this.AddToDeviceGroupAsync(
                    await this.GetMachineByIdAsync(machineId),
                    await this.deviceGroupRepository.GetAsync(deviceGroupId));
        }
       
        public virtual async Task AddToDeviceGroupAsync(Machine machine, DeviceGroup dg)
        {
            var currentOus = await this.GetDeviceGroupsAsync(machine);

            if (currentOus.Any(cou => cou.Id == dg.Id))
            {
                return;
            }

            await this.machineDeviceGroupRepository.InsertAsync(new MachineDeviceGroup(machine.Id, dg.Id, dg.Code));
        }

 
        public virtual async Task<List<DeviceGroup>> GetDeviceGroupsAsync(Machine machine)
        {
            var query = from uou in this.machineDeviceGroupRepository.GetAll()
                        join ou in this.deviceGroupRepository.GetAll() on uou.DeviceGroupId equals ou.Id
                        where uou.MachineId == machine.Id
                        select ou;

            return await query.ToListAsync();
        }

        public virtual async Task<List<FlatMachineDto>> GetInDeviceGroupMachines()
        {
            return (await this.GetMachineCacheItemAsync()).CatchedMachines.ToList();
        }


        public virtual async Task<List<FlatMachineDto>> GetInDeviceGroupMachinesWithInactiveAsync()
        {
            return (await this.GetAllMachineCacheItemAsync()).CatchedMachines.ToList();
        }

        public async Task<Machine> GetMachineByIdAsync(int id)
        {
            var machine = await this.machineRepository.FirstOrDefaultAsync(q => q.Id == id);
            if (machine == null)
            {
                throw new UserFriendlyException(this.L("DeviceIdDoesNotExist{0}", id));
            }

            return machine;
        }

        public string GetMachineImagePath(EntityDto<Guid?> input)
        {
            var imagePath = string.Empty;
            if (!input.Id.HasValue) return imagePath;

            var binaryObject = this.binaryObjectRepository.Get(input.Id.Value);
            if (binaryObject == null) return imagePath;

            var tempFileName = $"{input.Id}.jpg";
            var tempProfilePicturePath = Path.Combine(this.appFolders.TempFileDownloadFolder, tempFileName);
            if (!File.Exists(tempProfilePicturePath))
                using (Stream stream = new MemoryStream(binaryObject.Bytes))
                {
                    var imageFile = Image.FromStream(stream);
                    imageFile.Save(tempProfilePicturePath);
                }

            imagePath = $"Temp/Downloads/{tempFileName}";
            return imagePath;
        }


        public async Task<IEnumerable<NameValueDto<int>>> ListMachines()
        {
            return await this.machineRepository.GetAll().OrderBy(m =>  m.SortSeq).ThenBy(m=> m.Code)
                .Select(m => new NameValueDto<int> { Name = m.Name, Value = m.Id })
                .ToListAsync();
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListOrderedMachines()
        {
            var query
                = await (from mdg in this.machineDeviceGroupRepository.GetAll()
                         join m in this.machineRepository.GetAll() on mdg.MachineId equals m.Id
                         orderby mdg.DeviceGroupId, m.SortSeq, m.Code
                         select new NameValueDto<int>()
                         {
                             Name = m.Name,
                             Value = m.Id
                         }).ToListAsync();
            return query;
        }


        public async Task<IEnumerable<NameValueDto<int>>> ListDefaultSearchMachines()
        {
            var defaultMachineCount = AppSettings.DefaultSearchMachineCount;

            var query = await (from m in machineRepository.GetAll()
                               join mdg in machineDeviceGroupRepository.GetAll() on m.Id equals mdg.MachineId
                               select new
                               {
                                   mdg.DeviceGroupId,
                                   m.SortSeq,
                                   m.Code,
                                   m.Name,
                                   m.Id
                               }).OrderBy(t=>t.DeviceGroupId).ThenBy(t=>t.SortSeq).ThenBy(t=>t.Code).Take(defaultMachineCount).ToListAsync();

            return query.Select(t => new NameValueDto<int> { Name = t.Name, Value =t.Id});

        }

        public IEnumerable<Machine> ListMachinesInDeviceGroup(string workShopCode)
        {
            var targetGroup = deviceGroupRepository.FirstOrDefault(q => q.Code == workShopCode);
            if (targetGroup == null)
            {
                throw new UserFriendlyException(this.L("WorkshopDoesNotExist"));
            }

            List<Machine> machines = new List<Machine>();

            var targetMachines
                = from mdg in this.machineDeviceGroupRepository.GetAll()
                  join m in this.machineRepository.GetAll() on mdg.MachineId equals m.Id
                  where mdg.DeviceGroupCode.StartsWith(targetGroup.Code)
                  orderby mdg.DeviceGroupId, m.SortSeq, m.Code
                  select m;

            foreach (var machine in targetMachines)
            {
                machines.Add(machine);
            }

            //machines.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));

            return machines.Distinct();
        }

        public virtual async Task<bool> IsInDeviceGroupAsync(int machineId, int deviceGroupId)
        {
            return
                await
                this.IsInDeviceGroupAsync(
                    await this.GetMachineByIdAsync(machineId),
                    await this.deviceGroupRepository.GetAsync(deviceGroupId));
        }

        public virtual async Task<bool> IsInDeviceGroupAsync(Machine machine, DeviceGroup dg)
        {
            return
                await
                this.machineDeviceGroupRepository.CountAsync(
                    uou => uou.MachineId == machine.Id && uou.DeviceGroupId == dg.Id) > 0;
        }

        public virtual async Task RemoveFromDeviceGroupAsync(int machineId, int deviceGroupId)
        {
            await
                this.RemoveFromDeviceGroupAsync(
                    await this.GetMachineByIdAsync(machineId),
                    await this.deviceGroupRepository.GetAsync(deviceGroupId));
        }

        public virtual async Task RemoveFromDeviceGroupAsync(Machine machine, DeviceGroup dg)
        {
            await
                this.machineDeviceGroupRepository.DeleteAsync(
                    uou => uou.MachineId == machine.Id && uou.DeviceGroupId == dg.Id);

            await this.deviceGroupYieldMachineRepository.DeleteAsync(dgy => dgy.MachineId == machine.Id);
        }

        public async Task VerifyOperationPermission(int machineId)
        {
            var authorizedDeviceGroupIds = await this.roleManager.GetGrantedDeviceGroupPermissionsAsync();

            var query = from m in machineRepository.GetAll()
                        join md in machineDeviceGroupRepository.GetAll() on m.Id equals md.MachineId
                        where authorizedDeviceGroupIds.Contains(md.DeviceGroupId) && m.Id == machineId
                        select m;

            if (!query.Any())
            {
                throw new UserFriendlyException(this.L("NoRightToOperateThisEquipment"));
            }
        }

        public async Task<List<Machine>> ListMachinesInPermissionButNotInArray(int[] ignoreMachines)
        {
            var authorizedDeviceGroupIds = await this.roleManager.GetGrantedDeviceGroupPermissionsAsync();

            var query = from m in machineRepository.GetAll()
                        join md in machineDeviceGroupRepository.GetAll() on m.Id equals md.MachineId
                        where authorizedDeviceGroupIds.Contains(md.DeviceGroupId) && !ignoreMachines.Contains(m.Id)
                        select m;

            return query.ToList();
        }
        private async Task<MachineCacheItem> GetMachineCacheItemAsync()
        {
            var cacheKey = MachineCacheItem.CacheStoreName;
            return await this.cacheManager.GetMachineCache().GetAsync(
                cacheKey,
                async () =>
                {
                    var newCacheItem = new MachineCacheItem();
                    var machines = (from m in this.machineRepository.GetAll().Where(p => p.IsActive)
                                    join mg in this.machineDeviceGroupRepository.GetAll() on m.Id equals
                                        mg.MachineId
                                    orderby m.SortSeq,m.Code
                                    select
                                        new FlatMachineDto
                                        {
                                            Id = m.Id,
                                            Code = m.Code,
                                            IsActive = m.IsActive,
                                            Name = m.Name,
                                            Desc = m.Desc,
                                            DeviceGroupId = mg.DeviceGroupId,
                                            ImageId = m.ImageId,
                                            SortSeq = m.SortSeq
                                        }).ToArray();
                    
                    foreach (var machineCachedDto in machines)
                    {
                        newCacheItem.CatchedMachines.Add(machineCachedDto);
                    }

                    return await Task.FromResult(newCacheItem);
                });
        }
       
        private async Task<MachineCacheItem> GetAllMachineCacheItemAsync()
        {
            var cacheKey = MachineCacheItem.CacheAllStoreName;
            return await this.cacheManager.GetMachineCache().GetAsync(
                cacheKey,
                async () =>
                {
                    var newCacheItem = new MachineCacheItem();
                    var machines = (from m in this.machineRepository.GetAll()
                                    join mg in this.machineDeviceGroupRepository.GetAll() on m.Id equals
                                        mg.MachineId
                                    select
                                        new FlatMachineDto
                                        {
                                            Id = m.Id,
                                            Code = m.Code,
                                            IsActive = m.IsActive,
                                            Name = m.Name,
                                            Desc = m.Desc,
                                            DeviceGroupId = mg.DeviceGroupId,
                                            ImageId = m.ImageId,
                                            SortSeq = m.SortSeq
                                        }).ToArray();

                    foreach (var machineCachedDto in machines)
                    {
                        newCacheItem.CatchedMachines.Add(machineCachedDto);
                    }

                    return await Task.FromResult(newCacheItem);
                });
        }

        public async Task DeleteMachine(Machine machine)
        {
            await this.machineRepository.DeleteAsync(machine);

            this.dmpManager.Delete(machine.DmpMachineId, machine.Id);

            await this.DeleteMachineCascadeInfoById(machine.Id);

            mongoMachineManager.Delete(machine);
        }

        private async Task DeleteMachineCascadeInfoById(int machineId)
        {
            var nowDate = DateTime.Today;
            var nowDateShiftRange = await this.shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange(machineId,nowDate);
            var shiftDay = nowDateShiftRange.Any(s=>DateTime.Now>= Convert.ToDateTime(s.BeginTime)) ? nowDate : nowDate.AddDays(-1);
            var request = new MachineShiftDeleteDto {MachineId = machineId,ShiftDay = shiftDay };

            await this.machinesShiftDetailsManager.DeleteShiftDetailsByMachine(request);
            await this.machineShiftEffectiveIntervalManager.DeleteByMachine(request);
            await this.traceFlowSettingManager.RemoveMachineFromFlowSettingById(machineId);
            if (await this.performancePersonnelOnDeviceRepository.GetAll().AnyAsync(p=>p.MachineId == machineId))
            {
                await this.performanceDeviceManager.Offline(machineId, AbpSession.UserId ?? 0);
            }
            await this.reasonFeedbackManager.ForcedFinishByMachine(machineId, AbpSession.UserId);
        }
    }
}