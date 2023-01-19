namespace Wimi.BtlCore.StaffPerformances
{
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Collections.Extensions;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Authorization.Users;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.StaffPerformance;
    using Wimi.BtlCore.StaffPerformances.Dto;

    [AbpAuthorize(PermissionNames.Pages_StaffPerformance_OnlineOrOffline)]
    public class PerformanceDeviceAppService : BtlCoreAppServiceBase, IPerformanceDeviceAppService
    {
        private readonly IPerformancePersonnelManager performancePersonnelManager;

        private readonly IPerformanceDeviceManager performanceDeviceManager;
        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDevcieRepository;
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IRepository<MachinesShiftDetail> machineShiftDetailRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;

        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;

        private readonly IMachineManager machineManager;
        private readonly IRepository<User, long> userRepository;

        private readonly IRepository<Machine> machineRepository;

        public PerformanceDeviceAppService(
            IPerformanceDeviceManager performanceDeviceManager,
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDevcieRepository,
            ICommonLookupAppService commonLookupAppService,
            IRepository<MachinesShiftDetail> machineShiftDetailRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IPerformancePersonnelManager performancePersonnelManager,
            IRepository<User, long> userRepository,
            IMachineManager machineManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<Machine> machineRepository)
        {
            this.performancePersonnelOnDevcieRepository = performancePersonnelOnDevcieRepository;
            this.performanceDeviceManager = performanceDeviceManager;
            this.commonLookupAppService = commonLookupAppService;
            this.machineShiftDetailRepository = machineShiftDetailRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.performancePersonnelManager = performancePersonnelManager;
            this.userRepository = userRepository;
            this.machineManager = machineManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
        }
        [HttpPost]
        public async Task<ListResultDto<GetDevicesRequestDto>> GetDevices(GetDevicesDto input)
        {
            // 获取已经授权的设备和设备组
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var query = from m in deviceGroupAndMachineWithPermissions.Machines
                        join p in this.performancePersonnelOnDevcieRepository.GetAll() on m.Id equals p.MachineId into g
                        from k in g.DefaultIfEmpty()
                        let user = this.userRepository.GetAll()
                        select new GetDevicesRequestDto
                        {
                            MachineId = m.Id,
                            MachineName = m.Name,
                            DeviceGroupId = m.DeviceGroupId,
                            OnlineDate = k?.OnlineDate.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                            PersonnelName = this.GetUserPersonnelName(k, user),
                            PersonnelUserId = k != null ? user.First(u => u.Id == k.UserId).Id : (long?)null,
                            MachineImageSrc = this.GetMachineImagePath(m.ImageId),
                            IsOnline = k != null,
                            Id = k?.Id ?? 0
                        };

            // 过滤查询
            switch (input.UseState)
            {
                case EnumDeviceUseState.Online:
                    {
                        query = query.Where(p => p.IsOnline);
                        break;
                    }

                case EnumDeviceUseState.Offline:
                    {
                        query = query.Where(p => !p.IsOnline);
                        break;
                    }

                case EnumDeviceUseState.Mine:
                    {
                        query = query.Where(p => p.PersonnelUserId == this.AbpSession.UserId);
                        break;
                    }

                case EnumDeviceUseState.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            query = query.WhereIf(input.DeviceGroupIds.Length == 0 && input.DeviceIds.Length >= 0, p => input.DeviceIds.Contains(p.MachineId))
                         .WhereIf(input.DeviceGroupIds.Length > 0, p => input.DeviceGroupIds.Contains(p.DeviceGroupId));

            // 去重复数据：一个设备属于多个设备组
            var resutl = new List<GetDevicesRequestDto>();
            var getDevicesRequestDtos = query as GetDevicesRequestDto[] ?? query.ToArray();
            if (getDevicesRequestDtos.Any())
            {
                resutl = getDevicesRequestDtos.GroupBy(o => o.MachineId).Select(g => g.First()).ToList();
            }

            return new ListResultDto<GetDevicesRequestDto>(resutl);
        }

        /// <summary>
        /// 获取设备班次详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<NameValueDto<int>>> GetMachineShiftDetail(EntityDto input)
        {
            // 设备班次详细
            var shiftDetailQueryable = this.machineShiftDetailRepository.GetAll().Where(m => m.MachineId == input.Id && m.ShiftDay == DateTime.Today);

            // 解决方案条目
            var solutionItemQueryable = this.shiftSolutionItemRepository.GetAll();

            var query = await shiftDetailQueryable.Join(solutionItemQueryable, d => d.ShiftSolutionItemId, s => s.Id, (d, s) => new NameValueDto<int>() { Name = s.Name, Value = d.Id }).ToListAsync();

            return new ListResultDto<NameValueDto<int>>(query);
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListShiftDetailByDeviceGroupId(EntityDto input)
        {
            var machine = this.machineDeviceGroupRepository.GetAll().Where(m => m.DeviceGroupId == input.Id).FirstOrDefault();
            var shiftDetailQueryable = this.machineShiftDetailRepository.GetAll().Where(m => m.MachineId == machine.MachineId && m.ShiftDay == DateTime.Today);

            var solutionItemQueryable = this.shiftSolutionItemRepository.GetAll();

            var query = await shiftDetailQueryable.Join(solutionItemQueryable, d => d.ShiftSolutionItemId, s => s.Id, (d, s) => new NameValueDto<int>() { Name = s.Name, Value = d.Id }).ToListAsync();

            return query;

        }

        [AbpAuthorize]
        public async Task Online(PersonnelOnDeviceDto input)
        {

            if (await this.performancePersonnelManager.DeviceIsUsed(input.MachineId))
            {
                await this.performanceDeviceManager.Offline(input.MachineId, input.UserId);
            }
            await this.performanceDeviceManager.Online(input.MachineId, input.UserId, input.ShiftId);

        }

        [AbpAuthorize]
        public async Task<string> OnlineAll(PersonnelOnDeviceDto input)
        {
            List<string> noShiftMachineIdList = new List<string>();
            string message = null;
            var allMachineId = await (from mdg in this.machineDeviceGroupRepository.GetAll()
                                      join m in this.machineRepository.GetAll() on mdg.MachineId equals m.Id
                                      where (mdg.DeviceGroupId == input.DeviceGroupId)
                                      select new
                                      {
                                          m.Name,
                                          mdg.MachineId
                                      }).ToListAsync();

            var shiftMachineId = await this.machineDeviceGroupRepository.GetAll()
               .Join(this.machineShiftDetailRepository.GetAll(), d => d.MachineId, s => s.MachineId,
                   (d, s) => new { mdg = d, msd = s }).Where(d => d.mdg.DeviceGroupId == input.DeviceGroupId)
               .Select(a => a.mdg.MachineId).ToListAsync();

            var selectedShiftItemId = this.machineShiftDetailRepository.FirstOrDefault(s => s.Id == input.ShiftId).ShiftSolutionItemId;

            var selectedShiftItem = this.shiftSolutionItemRepository.FirstOrDefault(s => s.Id == selectedShiftItemId);

            foreach (var m in allMachineId)
            {
                if (shiftMachineId.Contains(m.MachineId))
                     
                {
                    if (await this.performancePersonnelManager.DeviceIsUsed(m.MachineId))
                    {
                        await this.performanceDeviceManager.Offline(m.MachineId, input.UserId);
                    }

                    var shift = await this.GetMachineShiftDetail(new EntityDto() { Id = m.MachineId });

                    await this.performanceDeviceManager.Online(m.MachineId, input.UserId, shift.Items.FirstOrDefault(s => s.Name == selectedShiftItem.Name).Value);
                }
                else
                {
                    noShiftMachineIdList.Add(m.Name);
                    message =  noShiftMachineIdList.JoinAsString(",");
                }
            }
            return message;
        }


        /// <summary>
        /// 下线动作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Offline(PersonnelOnDeviceDto input)
        {
            await this.performanceDeviceManager.Offline(input.MachineId, input.UserId);
        }

        public async Task<string> OfflineBatch(PersonnelOnDeviceDto input)
        {
            var machienOfflineList = await (from mdg in this.machineDeviceGroupRepository.GetAll()
                                            join ppod in this.performancePersonnelOnDevcieRepository.GetAll() on mdg.MachineId equals ppod.MachineId
                                            where mdg.DeviceGroupId == input.DeviceGroupId && ppod.UserId == input.UserId
                                            select new
                                            {
                                                mdg.MachineId,
                                                ppod.UserId

                                            }).ToListAsync();
            if (machienOfflineList.IsNullOrEmpty())
            {
                return this.L("NoNeedToRepeatOffline");
            }
            foreach (var machine in machienOfflineList)
            {
                await this.performanceDeviceManager.Offline(machine.MachineId, machine.UserId);
            }

            return null;
        }

        private string GetMachineImagePath(Guid? imageId)
        {
            return this.machineManager.GetMachineImagePath(new EntityDto<Guid?>(imageId));
        }

        private string GetUserPersonnelName(PerformancePersonnelOnDevice device, IQueryable<User> user)
        {
            var name = string.Empty;
            if (device == null)
            {
                return name;
            }

            var entity = user.First(u => u.Id == device.UserId);
            name = $"{entity.Name}";

            return name;
        }
    }

}
