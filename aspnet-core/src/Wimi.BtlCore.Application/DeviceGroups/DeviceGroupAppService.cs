using Castle.Core.Internal;

namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.AutoMapper;
    using Abp.Collections.Extensions;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.DeviceGroups.Dto;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Plan.Manager;
    using Wimi.BtlCore.Trace.Manager;
    using System.Linq.Dynamic.Core;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Common;

    [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups)]
    public class DeviceGroupAppService : BtlCoreAppServiceBase, IDeviceGroupAppService
    {
        private readonly DeviceGroupManager deviceGroupManager;

        private readonly IRepository<DeviceGroup> deviceGroupRepository;

        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;

        private readonly MachineManager machineManager;

        private readonly IRepository<Machine> machineRepository;

        private readonly TraceFlowSettingManager traceFlowSettingManager;

        private readonly ProcessPlanManager processPlanManager;
        private readonly ICommonLookupAppService commonLookupAppService;
        public DeviceGroupAppService(
            DeviceGroupManager deviceGroupManager,
            MachineManager machineManager,
            TraceFlowSettingManager traceFlowSettingManager,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<Machine> machineRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            ProcessPlanManager processPlanManager,
            ICommonLookupAppService commonLookupAppService)
        {
            this.machineManager = machineManager;
            this.deviceGroupManager = deviceGroupManager;
            this.traceFlowSettingManager = traceFlowSettingManager;
            this.deviceGroupRepository = deviceGroupRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.processPlanManager = processPlanManager;
            this.commonLookupAppService = commonLookupAppService;
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageMembers)]
        public async Task<string> AddMachineListToDeviceGroup(MachineListToDeviceGroupInputDto input)
        {
            List<int> addedMachineIdList=new List<int>();
            List<string>addedMachineNameList=new List<string>();
            string message = null;
            foreach (var machineId in input.MachineIdList)
            {
                var alreadyAdded = await this.machineDeviceGroupRepository.GetAll().AnyAsync(m => m.MachineId == machineId);

                if (!alreadyAdded)
                {
                    await machineManager.AddToDeviceGroupAsync(machineId, input.DeviceGroupId);
                }
                else
                {
                    addedMachineIdList.Add(machineId);
                }
            }

            var machines = await this.machineRepository.GetAll().Where(m => input.MachineIdList.Contains(m.Id))
                .ToListAsync();

            foreach (var id in addedMachineIdList)
            {
                var machine = machines.FirstOrDefault(m => m.Id == id);
                if (machine != null) addedMachineNameList.Add(machine.Name);
                message = this.L("MachinesHasBeenInOtherDeviceGroup{0}", addedMachineNameList.JoinAsString(","));
            }

            return message;
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageMembers)]
        public async Task AddMachineToDeviceGroup(MachineToDeviceGroupInputDto input)
        {
            var alreadyAdded = this.machineDeviceGroupRepository.GetAll().Where(m => m.MachineId == input.MachineId)
                .Select(m => m.DeviceGroupId);
            if (alreadyAdded.IsNullOrEmpty())
            {
                await machineManager.AddToDeviceGroupAsync(input.MachineId, input.DeviceGroupId);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public async Task<DeviceGroupDto> CreateDeviceGroup(CreateDeviceGroupInputDto input)
        {
            var deviceGroup = new DeviceGroup(input.DisplayName, input.ParentId);
            deviceGroup.SetDmpGroupId();
            deviceGroup.Seq = input.Seq;
            await deviceGroupManager.CreateAsync(deviceGroup);

            // 页面需要保存后的Id，手动保存一次
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<DeviceGroupDto>(deviceGroup);
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public async Task DeleteDeviceGroup(EntityDto input)
        {
            await deviceGroupManager.DeleteAsync(input.Id);
        }

        [HttpPost]
        public async Task<PagedResultDto<DeviceGroupMachineListDto>> GetDeviceGroupMachines(
            GetDeviceGroupMachinesInputDto input)
        {
            var query = from uou in machineDeviceGroupRepository.GetAll()
                        join ou in deviceGroupRepository.GetAll() on uou.DeviceGroupId equals ou.Id
                        join m in machineRepository.GetAll() on uou.MachineId equals m.Id
                        where uou.DeviceGroupId == input.Id
                        select new DeviceGroupMachineListDto
                        {
                            Id = m.Id,
                            Code = m.Code,
                            Name = m.Name,
                            Desc = m.Desc,
                            SortSeq = m.SortSeq,
                            AddedTime = uou.CreationTime
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<DeviceGroupMachineListDto>(totalCount, items);
        }

        [HttpPost]
        public async Task<ListResultDto<DeviceGroupDto>> GetDeviceGroups()
        {
            var returnValue = new ListResultDto<DeviceGroupDto>();
            var deviceGroups = await deviceGroupRepository.GetAll().ToListAsync();

            return new ListResultDto<DeviceGroupDto>(
                deviceGroups.Select(
                        deviceGroup =>
                        {
                            var dto = new DeviceGroupDto();
                            ObjectMapper.Map(deviceGroup, dto);
                            dto.MemberCount = GetMemberCount(deviceGroup.Id);
                            return dto;
                        })
                    .ToList());
        }


        private int  GetMemberCount(int devicegroupId)
        {
            return machineDeviceGroupRepository.GetAll().Where(d => d.DeviceGroupId == devicegroupId).Count();
        }

        [AbpAllowAnonymous]
        public async Task<IEnumerable<DeviceGroupDto>> ListFirstClassDeviceGroups()
        {
            var deviceGroup= await this.deviceGroupManager.ListFirstClassDeviceGroups();
            return ObjectMapper.Map<IEnumerable<DeviceGroupDto>>(deviceGroup);
        }
        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageMembers)]
        public async Task<bool> IsInDeviceGroup(MachineToDeviceGroupInputDto input)
        {
            return await machineManager.IsInDeviceGroupAsync(input.MachineId, input.DeviceGroupId);
        }

        public List<NameValueDto> ListRootDevices()
        {
            return deviceGroupRepository
                .GetAll()
                .Where(q => !q.ParentId.HasValue)
                .OrderBy(q => q.Seq)
                .Select(q => new NameValueDto { Value = q.Id.ToString(),  Name = q.DisplayName })
                .ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public async Task<DeviceGroupDto> MoveDeviceGroup(MoveDeviceGroupInputDto input)
        {
            await deviceGroupManager.MoveAsync(input.Id, input.NewParentId);

            return await PackDeviceGroupDto(await deviceGroupRepository.GetAsync(input.Id));
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageMembers)]
        public async Task RemoveMachineFromDeviceGroup(MachineToDeviceGroupInputDto input)
        {
            await machineManager.RemoveFromDeviceGroupAsync(input.MachineId, input.DeviceGroupId);
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public async Task<DeviceGroupDto> UpdateDeviceGroup(UpdateDeviceGroupInputDto input)
        {
            var deviceGroup = await deviceGroupRepository.GetAsync(input.Id);

            deviceGroup.DisplayName = input.DisplayName;
            deviceGroup.Seq = input.Seq;

            await deviceGroupManager.UpdateAsync(deviceGroup);

            return await PackDeviceGroupDto(deviceGroup);
        }

        private async Task<DeviceGroupDto> PackDeviceGroupDto(DeviceGroup deviceGroup)
        {
            var dto = ObjectMapper.Map<DeviceGroupDto>(deviceGroup);
            dto.MemberCount =
                await machineDeviceGroupRepository.CountAsync(uou => uou.DeviceGroupId == deviceGroup.Id);
            return dto;
        }
    }
}