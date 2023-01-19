using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.Common.Dto;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Dmps;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Editions;
using Wimi.BtlCore.FmsCutters;
using Wimi.BtlCore.Order.Crafts;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Order.Processes.Dtos;

namespace Wimi.BtlCore.Common
{
    [AbpAllowAnonymous]
    public class CommonLookupAppService : BtlCoreAppServiceBase, ICommonLookupAppService
    {
        private readonly IRepository<CraftProcess> craftProcessRepository;

        private readonly DeviceGroupManager deviceGroupManager;

        private readonly EditionManager editionManager;

        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;

        private readonly MachineManager machineManager;

        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<Process> processRepository;

        private readonly IRepository<FmsCutters.FmsCutter> fmscutterRepository;

        private readonly IRepository<DmpMachine> dmpMachineRepository;

        private readonly RoleManager roleManager;

        public CommonLookupAppService(
            EditionManager editionManager,
            IRepository<Machine> machineRepository,
            MachineManager machineManager,
            RoleManager roleManager,
            DeviceGroupManager deviceGroupManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<Process> processRepository,
            IRepository<CraftProcess> craftProcessRepository,
            IRepository<FmsCutters.FmsCutter> fmscutterRepository,
            IRepository<DmpMachine> dmpMachineRepository)
        {
            this.editionManager = editionManager;
            this.machineRepository = machineRepository;
            this.machineManager = machineManager;
            this.roleManager = roleManager;
            this.deviceGroupManager = deviceGroupManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.processRepository = processRepository;
            this.craftProcessRepository = craftProcessRepository;
            this.fmscutterRepository = fmscutterRepository;
            this.dmpMachineRepository = dmpMachineRepository;
        }

        [HttpPost]
        public async Task<PagedResultDto<NameValueDto>> FindMachines(FindMachinesInputDto input)
        {
            var exceptMachineIds = new List<int>();

            exceptMachineIds = await machineDeviceGroupRepository.GetAll()
              .Select(m => m.MachineId)
              .ToListAsync();

            var query = this.machineRepository.GetAll()
                .Where(m => exceptMachineIds.All(g => g != m.Id))
                .Where(q => q.IsActive)
                .WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), u => u.Name.Contains(input.Search.Value)).AsEnumerable();

            var machineCount =   query.Count();

            //var machines =   query.OrderBy(u => new { u.SortSeq, u.Code }).ToList();

            var machines = query.ToList();

            //var machineList = ObjectMapper.Map<IEnumerable<NameValueDto>>(machines.Select(u => u.ToNameValue())).ToList();

            var machineList = new List<NameValueDto>();

            var nameValueList = machines.Select(u => u.ToNameValue());

            ObjectMapper.Map(nameValueList, machineList);

            return new DatatablesPagedResultOutput<NameValueDto>(machineCount, machineList, machineCount);
        }

        [HttpPost]
        public async Task<PagedResultDto<NameValueDto>> FindMachinesToDmp(FindMachinesInputDto input)
        {
            var exceptMachineIds = new List<int>();

            exceptMachineIds = await dmpMachineRepository.GetAll()
              .Select(m => m.MachineId)
              .ToListAsync();

            var query = this.machineRepository.GetAll()
                .Where(m => exceptMachineIds.All(g => g != m.Id))
                .Where(q => q.IsActive)
                .WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), u => u.Name.Contains(input.Search.Value)).AsEnumerable();

            var machineCount = query.Count();

            //var machines =   query.OrderBy(u => new { u.SortSeq, u.Code }).ToList();

            var machines = query.ToList();

            //var machineList = ObjectMapper.Map<IEnumerable<NameValueDto>>(machines.Select(u => u.ToNameValue())).ToList();

            var machineList = new List<NameValueDto>();

            var nameValueList = machines.Select(u => u.ToNameValue());

            ObjectMapper.Map(nameValueList, machineList);

            return new DatatablesPagedResultOutput<NameValueDto>(machineCount, machineList, machineCount);
        }

        public async Task<PagedResultDto<ProcessDto>> FindProcesses(FindProcessInputDto input)
        {
            var craftProcessIds = await this.craftProcessRepository.GetAll()
                                      .Where(s => s.CraftId == input.CraftId)
                                      .Select(s => s.ProcessId)
                                      .ToListAsync();

            var query = this.processRepository.GetAll()
                .Where(s => !craftProcessIds.Contains(s.Id))
                .WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), u => u.Name.Contains(input.Search.Value));

            var craftCount = await query.CountAsync();


            var crafts = ObjectMapper.Map<List<ProcessDto>>((await query.OrderBy(u => u.Code).ToListAsync()));

            return new DatatablesPagedResultOutput<ProcessDto>(craftCount, crafts, craftCount);
        }

        public async Task<DatatablesPagedResultOutput<NameValueDto>> FindUsers(FindUsersInputDto input)
        {
            if (this.AbpSession.TenantId != null) input.TenantId = this.AbpSession.TenantId;

            using (this.CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = this.UserManager.Users.WhereIf(
                    !input.Search.Value.IsNullOrWhiteSpace(),
                    u => u.Name.Contains(input.Search.Value) 
                         || u.UserName.Contains(input.Search.Value) || u.EmailAddress.Contains(input.Search.Value)
                         );

                var userCount = await query.CountAsync();
                var users = await query.OrderBy(u => u.Name).ThenBy(u => u.Surname).PageBy(input).ToListAsync();
                var userList = users.Select(
                        u =>
                        {
                            var fullName = u.UserName == User.AdminUserName ? User.AdminUserName : u.FullName;
                            return new NameValueDto($"{fullName} ({u.EmailAddress})", u.Id.ToString());
                        })
                    .ToList();

                return new DatatablesPagedResultOutput<NameValueDto>(
                    userCount,
                    userList,
                    this.UserManager.Users.Count(),
                    input.Draw);
            }
        }

        /// <summary>
        ///     获取设备组，设备和已授权的设备组Id列表
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        [HttpPost]
        public async Task<DeviceGroupAndMachineWithPermissionsDto> GetDeviceGroupAndMachineWithPermissions()
        {
            var deviceGroupWithPermissions = await this.GetDeviceGroupWithPermissions();
            var machines = await this.machineManager.GetInDeviceGroupMachines();
            return new DeviceGroupAndMachineWithPermissionsDto(deviceGroupWithPermissions, machines);
        }

        /// <summary>
        ///     获取设备组和已授权的设备组Id列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DeviceGroupWithPermissionsDto> GetDeviceGroupWithPermissions()
        {
            var grantedDeviceGroupIds = (await this.roleManager.GetGrantedDeviceGroupPermissionsAsync()).ToList();
            var deviceGroups = await this.deviceGroupManager.GetCachedDeviceGroups();

            return new DeviceGroupWithPermissionsDto(grantedDeviceGroupIds, deviceGroups);
        }

        [HttpPost]
        public async Task<ListResultDto<ComboboxItemDto>> GetEditionsForCombobox()
        {
            var editions = await this.editionManager.Editions.ToListAsync();
            return new ListResultDto<ComboboxItemDto>(
                editions.Select(e => new ComboboxItemDto(e.Id.ToString(), e.DisplayName)).ToList());
        }


        /// <summary>
        ///     始终返回客户设备Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<MachineSettingListDto>> GetMachines(FindMachinesInputDto input)
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var query = await this.machineRepository.GetAll().Where(q => q.IsActive).OrderBy(q => q.SortSeq).ToListAsync();

                return new ListResultDto<MachineSettingListDto>(ObjectMapper.Map<List<MachineSettingListDto>>(query));
            }
        }

        /// <summary>
        ///     如果是租户，返回租户设备ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<MachineSettingListDto>> GetMachinesForTenantSpecific(FindMachinesInputDto input)
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var query = await this.machineRepository.GetAll().Where(q => q.IsActive).OrderBy(q => q.SortSeq).ToListAsync();

                return new ListResultDto<MachineSettingListDto>(ObjectMapper.Map<List<MachineSettingListDto>>(query));
            }
        }

        public async Task<PagedResultDto<NameValueDto<int>>> FindFmsCutters(FindFmsCuttersInput input)
        {
            var query = fmscutterRepository.GetAll().WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), u => u.CutterNo.Contains(input.Search.Value));

            var count = await query.CountAsync();

            var cutters = await query.OrderByDescending(u => u.CreationTime).ToListAsync();

            return new DatatablesPagedResultOutput<NameValueDto<int>>(count, cutters.Select(s => new NameValueDto<int>(s.CutterNo, s.Id)).ToList());
        }

        /// <summary>
        /// 获取设备组，默认数量设备和已授权的设备组Id列表
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceGroupAndMachineWithPermissionsDto> GetDeviceGroupAndDefaultCountMachineWithPermissions()
        {
            var deviceGroupWithPermissions = await this.GetDeviceGroupWithPermissions();
            var machines = await this.machineManager.GetInDeviceGroupMachines();

            var defaultMachineCount = AppSettings.DefaultSearchMachineCount;

            if (machines.Count > defaultMachineCount)
            {
                machines = machines.OrderBy(p => p.DeviceGroupId)
                    .ThenBy(p => p.SortSeq).ThenBy(p => p.Code).Take(defaultMachineCount).ToList();
            }
            return new DeviceGroupAndMachineWithPermissionsDto(deviceGroupWithPermissions, machines);
        }



        /// <summary>
        /// 获取默认查询设备数
        /// </summary>
        /// <returns></returns>
        public int DefaultSelectedMachineCount()
        {
            return AppSettings.DefaultSearchMachineCount;
        }
    }
}