using Abp.Collections.Extensions;
using System.Collections;

namespace Wimi.BtlCore.Shifts
{
    using Abp.Application.Services.Dto;
    using Abp.Configuration;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Events.Bus;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using Dapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Wimi.BtlCore;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Shift;
    using Wimi.BtlCore.Shift.Dtos;
    using Wimi.BtlCore.Shifts.Dto;

    public class ShiftAppService : BtlCoreAppServiceBase, IShiftAppService
    {
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository;
        private readonly IRepository<MachinesShiftDetail> shiftDetailRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IRepository<ShiftSolution> shiftSolutionRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IShiftDetailRepository machineShiftDetailRepository;
        private readonly IShiftSolutionManager shiftSolutionManager;
        private readonly IEventBus enventBus;
        private readonly IRepository<ShiftCalendar, long> shiftCalendarRepository;
        private readonly IShiftRepository shiftRepository;

        public ShiftAppService(
            IRepository<ShiftSolution> shiftSolutionRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<Machine> machineRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<MachinesShiftDetail> shiftDetailRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IShiftDetailRepository machineShiftDetailRepository,
            IShiftSolutionManager shiftSolutionManager,
            IEventBus enventBus,
            IRepository<ShiftCalendar, long> shiftCalendarRepository,
            ISettingManager settingManager,
            IShiftRepository shiftRepository
            )
        {
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.machineShiftEffectiveIntervalRepository = machineShiftEffectiveIntervalRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.shiftDetailRepository = shiftDetailRepository;
            this.unitOfWorkManager = unitOfWorkManager;
            this.machineShiftDetailRepository = machineShiftDetailRepository;
            this.shiftSolutionManager = shiftSolutionManager;
            this.enventBus = enventBus;
            this.shiftCalendarRepository = shiftCalendarRepository;
            this.shiftRepository = shiftRepository;
        }

        public async Task<string> CheckBeforeDeleteMachineShiftSolution(MachineShiftSolutionInputDto input)
        {
            // check当前关联班次是否有生效:
            if (await shiftRepository.CheckIfCurrentDayShiftIsWorking(input.MachineId,input.Id))
                return this.L("MachieShiftHasBeenEffective");

            return string.Empty;
        }

        /// <summary>
        ///     check所选设备组下设备是否已有关联班次
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfHasDevicesAssociatedShiftSolution(DeviceGroupShiftSolutionInputDto input)
        {
            var flag = false;
            var devices = (from dg in this.deviceGroupRepository.GetAll()
                           join mdg in this.machineDeviceGroupRepository.GetAll() on dg.Id equals mdg.DeviceGroupId
                           join d in this.machineRepository.GetAll() on mdg.MachineId equals d.Id
                           where dg.Id == input.DeviceGroupId
                           select d.Id).ToList();

            if (await this.machineShiftEffectiveIntervalRepository.GetAll()
                    .AnyAsync(
                        s => devices.Contains(s.MachineId)
                             && !(input.EndTime < s.StartTime || input.StartTime > s.EndTime))) flag = true;
            return flag;
        }

        /// <summary>
        ///     按设备组关联班次
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> CreateDeviceGroupShiftSolution(DeviceGroupShiftSolutionInputDto input)
        {
            var deviceIds = (await this.GetDeviceIdsByGroupId(input.DeviceGroupId)).ToList();

            for (var i = 0; i < deviceIds.Count(); i++)

                // 针对每个设备进行排班
                await this.CreateDeviceShiftSollution(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = deviceIds[i],
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = input.StartTime,
                        EndTime = input.EndTime
                    });

            return true;
        }

        /// <summary>
        ///     根据时间段进行对新增班次进行处理
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateDeviceShiftSollution(DeviceShiftSolutionInputDto input)
        {
            // 筛选出所有有交集的班次方案
            var crossSolutions = await this.machineShiftEffectiveIntervalRepository.GetAll().Where(
                                         o => o.MachineId == input.DeviceId
                                              && (input.StartTime == o.EndTime || input.EndTime == o.StartTime))
                                     .ToListAsync();

            if (crossSolutions.Count > 0)
            {
                foreach (var t in crossSolutions) await this.DetermineWhichProcessToChoose(t, input);
            }
            else
            {
                await this.CreateNewMachineShiftSolution(input);

                await this.CreateNewMachineShiftDay(input);
            }
        }

        /// <summary>
        /// 创建班次具体信息
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CreateShiftInfo(ShiftInfoInputDto input)
        {
            if (!IsTimeCrossed(input)) throw new UserFriendlyException(this.L("ShiftTimeDurationRangeIsCrossed"));

            // 保存班次具体信息
            foreach (var t in input.ShiftSolutionItems) await this.shiftSolutionItemRepository.InsertAsync(t);
        }

        /// <summary>
        /// 创建班次方案
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CreateShiftSolution(ShiftSolutionInputDto input)
        {
            if (!await this.ValidateShiftSolutionNameIsUnique(input))
                throw new UserFriendlyException(this.L("ShiftSolutionNameAlreadyExists"));

            var ss = new ShiftSolution { Name = input.Name };

            await this.shiftSolutionRepository.InsertAsync(ss);
        }

        /// <summary>
        ///     解除当前关联的设备班次
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteMachineShiftSolution(MachineShiftSolutionInputDto input)
        {
            // 1.获取对应的设备班次信息
            var s4M = await this.machineShiftEffectiveIntervalRepository.FirstOrDefaultAsync(s => s.Id == input.Id);

            // 2.check当前关联班次是否有生效:
            if (await shiftRepository. CheckIfCurrentDayShiftIsWorking(input.MachineId,input.Id))
            {
                // 删掉对应ShiftDetail表里的记录
                this.machineShiftDetailRepository.DeleteShiftDetailsAndCalender(s4M.MachineId, s4M.ShiftSolutionId, DateTime.Today, s4M.EndTime, EnumEqType.Lt);

                // 已经生效,更新当前班次结束时间为今天并把今天之后的排的班次全部删掉
                s4M.EndTime = DateTime.Today;
                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);

                await machineShiftEffectiveIntervalRepository.DeleteAsync(p => p.StartTime > DateTime.Today && p.MachineId == s4M.MachineId);
            }
            else
            {
                // 没有生效
                if (s4M.StartTime < DateTime.Now)
                {
                    // 1.班次开始时间小于当天,更新班次结束时间为前一天时间,删除今天和之后的排的班次全部删掉
                    s4M.EndTime = DateTime.Today.AddDays(-1);

                    await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);

                    // 2.删掉对应Shift表里的记录
                    this.machineShiftDetailRepository.DeleteShiftDetailsAndCalender(s4M.MachineId, s4M.ShiftSolutionId, s4M.EndTime);
                }
                else
                {
                    // 2.班次开始时间等于当天,删除设备班次方案并删除今天和之后的排的班次全部删掉
                    await this.machineShiftEffectiveIntervalRepository.DeleteAsync(s4M);

                    // 2.删掉对应Shift表里的记录
                    this.machineShiftDetailRepository.DeleteShiftDetailsAndCalender(s4M.MachineId, s4M.ShiftSolutionId, s4M.StartTime, null, EnumEqType.Gt);
                }
            }
        }

        /// <summary>
        /// 删除班次方案
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public void DeleteShiftSolution(EntityDto<int> input)
        {
            this.shiftSolutionManager.CheckIsInUsing((int)input.Id);
            this.enventBus.Trigger(new ShiftSolutionEventData((int)input.Id));
        }

        /// <summary>
        /// 删除班次具体信息
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task DeleteShiftSolutionItem(EntityDto input)
        {
            await this.shiftSolutionItemRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 修改班次方案
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<ShiftSolutionDto> EditShiftSolution(ShiftSolutionInputDto input)
        {
            var result = new ShiftSolutionDto();

            if (!await this.ValidateShiftSolutionNameIsUnique(input))
                throw new UserFriendlyException(this.L("ShiftSolutionNameAlreadyExists"));

            if (input.Id == null) return result;

            var ss = await this.shiftSolutionRepository.FirstOrDefaultAsync((int)input.Id);

            if (ss == null) throw new UserFriendlyException(this.L("RecordIsNotExists"));
            ss.Name = input.Name;

            await this.shiftSolutionRepository.UpdateAsync(ss);

            var query = from ss2 in this.shiftSolutionRepository.GetAll()
                        join si in this.shiftSolutionItemRepository.GetAll() on ss.Id equals si.ShiftSolutionId into joinSS
                        from si in joinSS.DefaultIfEmpty()
                        group ss2 by new { ss2.Id, ss2.Name } into gd
                        select new ShiftSolutionDto { Id = gd.Key.Id, Name = gd.Key.Name, MemberCount = gd.Count() };

            result = await query.Where(q => q.Id == input.Id).FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        ///     获取所有设备组
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<ShiftSolutionDto>> GetDeviceGroups()
        {
            var query = from d in this.deviceGroupRepository.GetAll() select new { d.Id, Name = d.DisplayName };

            var items = await query.ToListAsync();

            return new ListResultDto<ShiftSolutionDto>(
                items.Select(item => new ShiftSolutionDto { Id = item.Id, Name = item.Name }).ToList());
        }

        /// <summary>
        /// 获取设备历史班次信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<DeviceHistoryShiftInfoDto>> GetDeviceHistoryShiftInfo(DeviceHistoryShiftInfoInputDto input)
        {
            return await shiftRepository.GetDeviceHistoryShiftInfo(input.DeviceId);
        }

        /// <summary>
        ///     获取设备正在使用和未生效的班次方案信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Machine4ShiftSolutionDto> GetMachineShiftSolution(Machine4ShiftSolutionInputDto input)
        {
            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var machine4ShiftSolutionDetailQueryExpression =
                from item in this.machineShiftEffectiveIntervalRepository.GetAll()
                where item.EndTime >= currentDate && item.MachineId == input.Id
                orderby item.StartTime
                select new Machine4ShiftSolutionDetail
                {
                    Id = item.Id,
                    ShiftSolutionId = item.ShiftSolutionId,
                    Name = item.ShiftSolution.Name,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime
                };

            var machine4ShiftSolutionDetail = await machine4ShiftSolutionDetailQueryExpression.ToListAsync();

            var result = new Machine4ShiftSolutionDto
            {
                MachineId = input.Id,
                Machine4ShiftSolutionDetail = machine4ShiftSolutionDetail
            };

            return result;
        }

        /// <summary>
        ///     获取当前设备级关联的班次信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineShiftSolutionDto>> GetMachineShiftSolutions(MachineShiftSolutionInputDto input)
        {
            return await shiftRepository.GetMachineShiftSolutions(input.Ids,input.ShiftSolutionId,input.Start,input.Length,input.QueryType);
        }

        /// <summary>
        ///     获取班次具体信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<ShiftInfoDto>> GetShiftInfos(ShiftSolutionInputDto input)
        {
            if (input?.Id == null) return new DatatablesPagedResultOutput<ShiftInfoDto>();

            var query = from si in this.shiftSolutionItemRepository.GetAll()
                        where si.ShiftSolutionId == input.Id
                        select new ShiftInfoDto
                        {
                            ShiftSolutionId = si.ShiftSolutionId,
                            Id = si.Id,
                            Name = si.Name,
                            StartTime = si.StartTime,
                            EndTime = si.EndTime,
                            Duration = si.Duration,
                            IsNextDay = si.IsNextDay,
                            CreationTime = si.CreationTime
                        };

            var totalCount = await query.CountAsync();

            var result = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<ShiftInfoDto>(totalCount, result);
        }

        /// <summary>
        /// 获取班次具体信息用户弹出框显示
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// The ShiftInfos
        /// </returns>
        [HttpPost]
        public async Task<List<ShiftInfoDto>> GetShiftInfosForModal(ShiftSolutionInputDto input)
        {
            if (input?.Id == null) return new List<ShiftInfoDto>();

            var query = from si in this.shiftSolutionItemRepository.GetAll()
                        where si.ShiftSolutionId == input.Id
                        select new ShiftInfoDto
                        {
                            ShiftSolutionId = si.ShiftSolutionId,
                            Id = si.Id,
                            Name = si.Name,
                            StartTime = si.StartTime,
                            EndTime = si.EndTime,
                            Duration = si.Duration,
                            IsNextDay = si.IsNextDay,
                            CreationTime = si.CreationTime
                        };

            return await query.ToListAsync();
        }

        /// <summary>
        ///     根据Id获取班次方案
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ShiftSolutionDto> GetShiftSolutionForModal(ShiftSolutionInputDto input)
        {
            var dtoQuery = from item in this.shiftSolutionRepository.GetAll()
                           where item.Id == input.Id
                           select new ShiftSolutionDto { Id = item.Id, Name = item.Name };

            return await dtoQuery.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     获取已有的班次方案
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<ShiftSolutionDto>> GetShiftSolutions()
        {
            var query = from ss in await this.shiftSolutionRepository.GetAll().ToListAsync()
                        join si in await this.shiftSolutionItemRepository.GetAll().ToListAsync() on ss.Id equals si.ShiftSolutionId into g
                        select new { ss.Id, ss.Name, memberCount = g.Count() };

            var items = query.ToList();

            return new ListResultDto<ShiftSolutionDto>(
                items.Select(
                        item => new ShiftSolutionDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            MemberCount = item.memberCount
                        })
                    .ToList());
        }

        [HttpPost]
        public IEnumerable GetShiftName(int deviceGroupId)
        {
            var machineIds = this.machineDeviceGroupRepository.GetAll().Where(m => m.DeviceGroupId == deviceGroupId)
                .Select(m => m.MachineId).ToList();
            if (machineIds.IsNullOrEmpty())
            {
                return null;
            }

            var machineId = machineIds.FirstOrDefault();

            var result = (from mdg in this.machineDeviceGroupRepository.GetAll()
                          join msd in this.shiftDetailRepository.GetAll() on mdg.MachineId equals msd.MachineId
                          join ssi in this.shiftSolutionItemRepository.GetAll() on msd.ShiftSolutionId equals ssi.ShiftSolutionId
                          where mdg.DeviceGroupId == deviceGroupId && mdg.MachineId == machineId
                          group new { ssi.Id, ssi.Name, ssi.ShiftSolutionId } by new { ssi.Name, ssi.Id, ssi.ShiftSolutionId } into g
                          select new
                          {
                              g.Key.Name,
                              g.Key.ShiftSolutionId,
                              g.Key.Id
                          }).ToList();
            return result;
        }

        /// <summary>
        ///     按单个设备设置班次方案
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateMachineShiftSolution(Machine4ShiftSolutionInputDto input)
        {
            if (CheckIfMachineShiftSolutionIsCorssed(input)) throw new UserFriendlyException(this.L("SelectedTimeIntervalsIntersect"));

            // 验证当前排班第一天班次日时间段，是否与上一个排班最后一天班次日时间段有有交集
            if (await CheckIfCrossedWithLastedEffectiveShiftSolution(input)) throw new UserFriendlyException(this.L("SelectedTimeIntervalsIntersect2"));

            foreach (var t in input.Machine4ShiftSolutionInputDetail)
            {
                // 这个属于新增的
                if (t.Id == 0)
                {
                    await this.CreateDeviceShiftSollution(new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.Id,
                        ShiftSolutionId = t.ShiftSolutionId,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime
                    });
                }
                else
                {   //（修改这里的两个删除方法，使用sql删除 ）
                    // 这个属于修改的
                    // 1.获取原有记录
                    var s4M = await this.machineShiftEffectiveIntervalRepository.FirstOrDefaultAsync(t.Id);

                    await machineShiftDetailRepository.DeleteShiftOldData(s4M);


                    //如果不提交，会影响后续的新增排序，用的是Sql
                    await this.unitOfWorkManager.Current.SaveChangesAsync();

                    // 3.更新MachineShiftEffectiveInterval记录
                    if (!s4M.IsSanmeDay())   // 当天记录已生效，不能修改
                    {
                        s4M.ShiftSolutionId = t.ShiftSolutionId;
                    }

                    s4M.StartTime = s4M.StartTime == DateTime.Today ? s4M.StartTime : t.StartTime;
                    s4M.EndTime = t.EndTime;

                    await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);

                    // 4.新增新班次记录(修改只能看到正在运行的或者以后要运行的)
                    var shiftDayList = await this.shiftDetailRepository.
                        GetAll().
                        Where(d => d.MachineId == s4M.MachineId && d.ShiftDay >= t.StartTime && d.ShiftDay <= DateTime.Today).
                        Select(d => d.ShiftDay).ToListAsync();
                    for (var time = t.StartTime; time <= t.EndTime; time = time.AddDays(1))
                    {
                        if (!shiftDayList.Contains(time))
                        {
                            await this.CreateNewMachineShiftDay(new DeviceShiftSolutionInputDto
                            {
                                DeviceId = input.Id,
                                ShiftSolutionId = t.ShiftSolutionId,
                                StartTime = time,
                                EndTime = time
                            });
                        }
                    }
                }
            }
        }

        private async Task<bool> CheckIfCrossedWithLastedEffectiveShiftSolution(Machine4ShiftSolutionInputDto input)
        {
            // 当前班次方案信息
            var fisrtShiftSolution = input.Machine4ShiftSolutionInputDetail.OrderBy(s => s.StartTime).FirstOrDefault();

            var shiftSolutionItems = await this.shiftSolutionItemRepository.GetAll().Where(s => s.ShiftSolutionId == fisrtShiftSolution.ShiftSolutionId).OrderBy(s => s.StartTime).ToListAsync();

            // 获取此次排班首日班次时间时间段

            var startTime = $"{fisrtShiftSolution.StartTime.ToString("yyyy-MM-dd")} {shiftSolutionItems.FirstOrDefault().StartTime.ToString("HH:mm:ss")}";
            var endTime = $"{fisrtShiftSolution.StartTime.ToString("yyyy-MM-dd")} {shiftSolutionItems.FirstOrDefault().EndTime.ToString("HH:mm:ss")}";

            var result = this.machineShiftDetailRepository.CheckIfCrossedWithLastedEffectiveShiftSolution(startTime, endTime);

            return result.Any();
        }

        /// <summary>
        ///     更新班次具体信息
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        public async Task UpdateShiftInfo(ShiftInfoInputDto input)
        {
            if (!IsTimeCrossed(input)) throw new UserFriendlyException(this.L("ShiftTimeDurationRangeIsCrossed"));
            // 更新班次具体信息
            foreach (var t in input.ShiftSolutionItems)
            {
                if (t.Id == 0)
                    await this.shiftSolutionItemRepository.InsertAsync(t);
                else await this.shiftSolutionItemRepository.UpdateAsync(t);
            }

            //找到已经被删除的Id
            var solutionItems = await this.shiftSolutionItemRepository.GetAll().Where(t => t.ShiftSolutionId == input.ShiftSolutionId).AsNoTracking().ToListAsync();

            var deletedItems = solutionItems.Where(s => !input.ShiftSolutionItems.Select(k => k.Id).Contains(s.Id)).ToList();
            deletedItems.ForEach(async d => { await this.shiftSolutionItemRepository.DeleteAsync(d); });
        }
 
        /// <summary>
        ///     验证设备设置的班次时间端是否有重叠
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool CheckIfMachineShiftSolutionIsCorssed(Machine4ShiftSolutionInputDto input)
        {
            var tempDate = new List<int>();

            foreach (var t in input.Machine4ShiftSolutionInputDetail)
            {
                for (var st = t.StartTime; st <= t.EndTime; st = st.AddDays(1))
                {
                    tempDate.Add(Convert.ToInt32(st.ToString("yyyyMMdd")));
                }
            }

            // 计算数组中重复的项个数
            var result = from e in tempDate
                         group e by e
                         into gd
                         select new { Count = gd.Count(), Angle = gd.Sum(e => e) };

            // 如果有重复项,则说明时间有交叉
            return result.Any(i => i.Count > 1);
        }

        /// <summary>
        ///     验证班次具体信息设置的时间是否有交集
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsTimeCrossed(ShiftInfoInputDto input)
        {
            // 获取在界面上设置时间跨度最小单元,比如10:00,下一个最近的时间点是10:30,duration是30
            var duration = Convert.ToDouble(AppSettings.Shift.ShiftTimeDuration);

            // 获取根据duration,计算出一个跨度占360度中多少度数
            var timeSpanCount = 24 * 60 / duration;

            var angleForPerTimeSpan = (double)360 / timeSpanCount;

            var shiftAngles = new List<double>();

            foreach (var t in input.ShiftSolutionItems)
            {
                // 计算该班次开始角度和结束角度
                var startTime = new DateTime(t.StartTime.Year, t.StartTime.Month, t.StartTime.Day, 0, 0, 0);

                var startAngle = (t.StartTime - startTime).TotalMinutes / 4;

                var endAngle = (t.EndTime - startTime).TotalMinutes / 4;

                // 如果开始角度大于结束角度,即意味这个班次跨天了
                if (startAngle > endAngle)
                {
                    // 开始时间到明天的零点
                    for (var angle = startAngle; angle < 360; angle = angle + angleForPerTimeSpan)
                        shiftAngles.Add(angle);

                    // 明天零点到结束时间
                    for (double angle = 0; angle < endAngle; angle = angle + angleForPerTimeSpan)
                        shiftAngles.Add(angle);
                }
                else
                {
                    for (var angle = startAngle; angle < endAngle; angle = angle + angleForPerTimeSpan)
                        shiftAngles.Add(angle);
                }
            }

            // 计算数组中重复的项个数
            var result = from e in shiftAngles
                         group e by e
                         into gd
                         select new { Count = gd.Count(), Angle = gd.Sum(e => e) };

            // 如果有重复项超过一个,则说明时间有交叉
            return !result.Any(i => i.Count > 1);
        }

        /// <summary>
        ///     创建新的班次天数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task CreateNewMachineShiftDay(DeviceShiftSolutionInputDto input)
        {

            DeviceShiftSolutionInput deviceShiftSolution = new DeviceShiftSolutionInput();

            //input.MapTo(deviceShiftSolution);
            ObjectMapper.Map(input, deviceShiftSolution);

            deviceShiftSolution.CreatorUserId = AbpSession.UserId ?? 0;

            await this.machineShiftDetailRepository.InsertMachineShiftDetailsAsync(deviceShiftSolution);

            await this.machineShiftDetailRepository.InsertMachineShiftCalendarAsync(deviceShiftSolution);

            #region 循环插入设备排班
            /*
            for (var st = input.StartTime; st < input.EndTime; st = st.AddDays(1))
            {
                // 创建具体的班次信息
                // 1.获取班次方案的具体信息
                var shiftSolutionItems =
                    await
                    (from si in this.shiftSolutionItemRepository.GetAll()
                     where si.ShiftSolutionId == input.ShiftSolutionId
                     select si).ToListAsync();

                // 2.创建设备班次
                foreach (var t in shiftSolutionItems)
                {                   
                    var sd = new MachinesShiftDetails
                    {
                        MachineId = input.DeviceId,
                        ShiftDay = st,
                        ShiftSolutionId = input.ShiftSolutionId,
                        ShiftSolutionItemId = t.Id
                    };

                    await this.shiftDetailRepository.InsertAsync(sd);
                }
            }
            */
            #endregion
        }

        /// <summary>
        ///     创建新的设备班次方案
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task CreateNewMachineShiftSolution(DeviceShiftSolutionInputDto input)
        {
            var sm = new MachineShiftEffectiveInterval
            {
                ShiftSolutionId = input.ShiftSolutionId,
                MachineId = input.DeviceId,
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };

            await this.machineShiftEffectiveIntervalRepository.InsertAsync(sm);
        }

        /// <summary>
        ///     用于判断使用哪个Process来调整班次日期
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task DetermineWhichProcessToChoose(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            // 针对有重叠的班次根据时间段进行调整
            if (input.StartTime == s4M.EndTime)
            {
                await this.ConnectShift1(s4M, input);
            }
            else if (input.EndTime == s4M.StartTime)
            {
                await this.ConnectShift2(s4M, input);
            }

        }

        /// <summary>
        /// 原来|--------|
        /// 输入         |------|
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task ConnectShift1(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {  // 1.扩展班次天数

            if (input.StartTime <= DateTime.Today && input.EndTime > DateTime.Today)
            {
                await this.shiftDetailRepository.DeleteAsync(s => s.MachineId == input.DeviceId && (s.ShiftDay > DateTime.Today && s.ShiftDay < input.EndTime));

                await this.CreateNewMachineShiftDay(new DeviceShiftSolutionInputDto
                {
                    DeviceId = input.DeviceId,
                    ShiftSolutionId = input.ShiftSolutionId,
                    StartTime = DateTime.Today.AddDays(1),
                    EndTime = input.EndTime
                });

            }
            else
            {
                await this.shiftDetailRepository.DeleteAsync(s => s.MachineId == input.DeviceId && s.ShiftDay >= input.StartTime && s.ShiftDay < input.EndTime);

                await this.CreateNewMachineShiftDay(new DeviceShiftSolutionInputDto
                {
                    DeviceId = input.DeviceId,
                    ShiftSolutionId = input.ShiftSolutionId,
                    StartTime = input.StartTime,
                    EndTime = input.EndTime
                });

            }
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 2.更新开始时间
                s4M.EndTime = input.EndTime;

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                await this.CreateNewMachineShiftSolution(input);
            }

        }

        /// <summary>
        /// 原来       |-------|
        /// 输入|------|
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task ConnectShift2(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            // 1.扩展班次天数
            if (input.StartTime <= DateTime.Today && input.EndTime > DateTime.Today)
            {
                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId
                         && (s.ShiftDay > DateTime.Today && s.ShiftDay < input.EndTime));

                // 过去的天创建没什么意义，此处不再创
                await this.CreateNewMachineShiftDay(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = DateTime.Today.AddDays(1),
                        EndTime = input.EndTime
                    });
            }
            else
            {
                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay >= input.StartTime && s.ShiftDay < input.EndTime);
                await this.CreateNewMachineShiftDay(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = input.StartTime,
                        EndTime = input.EndTime
                    });
            }
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 2.更新开始时间
                s4M.StartTime = input.StartTime;

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                await this.CreateNewMachineShiftSolution(input);
            }
        }

        private async Task<IEnumerable<int>> GetDeviceIdsByGroupId(int id)
        {
            var deviceIds = await
                (from dg in this.deviceGroupRepository.GetAll()
                 join mdg in this.machineDeviceGroupRepository.GetAll() on dg.Id equals
                     mdg.DeviceGroupId
                 join d in this.machineRepository.GetAll() on mdg.MachineId equals d.Id
                 where dg.Id == id
                 select d.Id).ToListAsync();

            return deviceIds;
        }

        /// <summary>
        ///     |-----------|原班次
        ///     |           :
        ///     |--------------|新班次
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task Process1(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 1.扩展班次天数
                await this.CreateNewMachineShiftDay(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = input.StartTime,
                        EndTime = s4M.StartTime.AddDays(-1)
                    });

                // 2.更新开始时间
                s4M.StartTime = input.StartTime;

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                // 1.删掉原班次天数
                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay > s4M.StartTime && s.ShiftDay < input.EndTime);

                // 2.更新开始时间
                s4M.StartTime = input.EndTime.AddDays(1);

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);

                // 3.新增新班次方案记录
                await this.CreateNewMachineShiftSolution(input);

                // 4.新增新班次天数
                await this.CreateNewMachineShiftDay(input);
            }
        }

        /// <summary>
        ///     |-----------| 原班次
        ///     :           |
        ///     |--------------| 新班次
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task Process2(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 1.扩展原班次天数
                await this.CreateNewMachineShiftDay(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = s4M.EndTime.AddDays(1),
                        EndTime = input.EndTime
                    });

                // 2.更新开始时间
                s4M.EndTime = input.EndTime;

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                // 1.删掉已分配的班次天数
                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay >= input.StartTime && s.ShiftDay <= s4M.EndTime);

                // 2.更新结束时间
                s4M.EndTime = input.StartTime.AddDays(-1);

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);

                // 3.新增新班次方案记录
                await this.CreateNewMachineShiftSolution(input);

                // 4.新增新班次天数
                await this.CreateNewMachineShiftDay(input);
            }
        }

        /// <summary>
        ///     |--------| 原班次
        ///     |        |
        ///     |-----------| 新班次
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task Process3(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 1.扩展新开始时间到原始开始时间
                if (input.StartTime < s4M.StartTime)
                    await this.CreateNewMachineShiftDay(
                        new DeviceShiftSolutionInputDto
                        {
                            DeviceId = input.DeviceId,
                            ShiftSolutionId = input.ShiftSolutionId,
                            StartTime = input.StartTime,
                            EndTime = s4M.StartTime.AddDays(-1)
                        });

                // 2.扩展原有结束时间到新结束时间
                if (input.EndTime > s4M.EndTime)
                    await this.CreateNewMachineShiftDay(
                        new DeviceShiftSolutionInputDto
                        {
                            DeviceId = input.DeviceId,
                            ShiftSolutionId = input.ShiftSolutionId,
                            StartTime = s4M.EndTime.AddDays(1),
                            EndTime = input.EndTime
                        });

                // 3.更新开始时间和结束时间
                s4M.StartTime = input.StartTime;
                s4M.EndTime = input.EndTime;

                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                // 1.删掉原有设备班次方案记录
                await this.machineShiftEffectiveIntervalRepository.DeleteAsync(s4M);

                // 2.删掉已分配的班次天数
                await
                    this.shiftDetailRepository
                    .DeleteAsync(s => s.MachineId == input.DeviceId && s.ShiftDay >= s4M.StartTime && s.ShiftDay <= s4M.EndTime);

                // 3.创建新的设备班次方案
                await this.CreateNewMachineShiftSolution(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = input.StartTime,
                        EndTime = input.EndTime
                    });

                // 4.创建新的班次天数
                await this.CreateNewMachineShiftDay(
                    new DeviceShiftSolutionInputDto
                    {
                        DeviceId = input.DeviceId,
                        ShiftSolutionId = input.ShiftSolutionId,
                        StartTime = input.StartTime,
                        EndTime = input.EndTime
                    });
            }
        }

        /// <summary>
        ///     |-----------| 原班次
        ///     :           :
        ///     |---------| 新班次
        /// </summary>
        /// <param name="s4M"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task Process4(MachineShiftEffectiveInterval s4M, DeviceShiftSolutionInputDto input)
        {
            if (s4M.ShiftSolutionId == input.ShiftSolutionId)
            {
                // 1.删掉原有开始时间到新的开始时间-1
                var et = input.StartTime.AddDays(-1);

                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay >= s4M.StartTime && s.ShiftDay <= et);

                // 2.删掉新的结束时间+1到原有结束时间
                var st = input.EndTime.AddDays(1);

                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay >= st && s.ShiftDay <= s4M.EndTime);

                // 3.更新开始时间和结束时间
                s4M.StartTime = input.StartTime;
                s4M.EndTime = input.EndTime;
                await this.machineShiftEffectiveIntervalRepository.UpdateAsync(s4M);
            }
            else
            {
                // 1.删掉原有设备班次方案记录
                await this.machineShiftEffectiveIntervalRepository.DeleteAsync(s4M);

                // 2.删掉已分配的班次天数
                await this.shiftDetailRepository.DeleteAsync(
                    s => s.MachineId == input.DeviceId && s.ShiftDay >= s4M.StartTime && s.ShiftDay <= s4M.EndTime);
                await this.unitOfWorkManager.Current.SaveChangesAsync();

                // 3.构建原记录开始时间到新班次开始时间前一天的记录
                // 关联新增的设备班次
                var sm = new MachineShiftEffectiveInterval
                {
                    ShiftSolutionId = s4M.ShiftSolutionId,
                    MachineId = input.DeviceId,
                    StartTime = s4M.StartTime,
                    EndTime = input.StartTime.AddDays(-1)
                };

                await this.machineShiftEffectiveIntervalRepository.InsertAsync(sm);

                for (var st = sm.StartTime; st <= sm.EndTime; st = st.AddDays(1))
                {
                    // 创建具体的班次信息
                    // 1.获取班次方案的具体信息
                    var shiftSolutionItems = await
                        (from si in this.shiftSolutionItemRepository.GetAll()
                         where si.ShiftSolutionId == input.ShiftSolutionId
                         select si).ToListAsync();

                    foreach (var t in shiftSolutionItems)
                    {
                        var sd = new MachinesShiftDetail
                        {
                            MachineId = input.DeviceId,
                            ShiftDay = st,
                            ShiftSolutionId = s4M.ShiftSolutionId,
                            ShiftSolutionItemId = t.Id
                        };

                        await this.shiftDetailRepository.InsertAsync(sd);
                    }
                }

                // 4.构建原记录结束时间到新班次结束时间后一天的记录
                // 关联新增的设备班次
                var sm2 = new MachineShiftEffectiveInterval
                {
                    ShiftSolutionId = s4M.ShiftSolutionId,
                    MachineId = input.DeviceId,
                    StartTime = input.EndTime.AddDays(1),
                    EndTime = s4M.EndTime
                };

                await this.machineShiftEffectiveIntervalRepository.InsertAsync(sm2);

                for (var st = sm2.StartTime; st <= sm2.EndTime; st = st.AddDays(1))
                {
                    // 创建具体的班次信息
                    // 1.获取班次方案的具体信息
                    var shiftSolutionItems =
                        await
                        (from si in this.shiftSolutionItemRepository.GetAll()
                         where si.ShiftSolutionId == input.ShiftSolutionId
                         select si).ToListAsync();

                    foreach (var t in shiftSolutionItems)
                    {
                        var sd = new MachinesShiftDetail
                        {
                            MachineId = input.DeviceId,
                            ShiftDay = st,
                            ShiftSolutionId = s4M.ShiftSolutionId,
                            ShiftSolutionItemId = t.Id
                        };

                        await this.shiftDetailRepository.InsertAsync(sd);
                    }
                }

                // 5.创建新的设备班次方案记录
                await this.CreateNewMachineShiftSolution(input);

                // 6.创建新的设备班次天数
                await this.CreateNewMachineShiftDay(input);
            }
        }

        /// <summary>
        ///     验证班次方案名称是否已存在
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<bool> ValidateShiftSolutionNameIsUnique(ShiftSolutionInputDto input)
        {
            var flag = await this.shiftSolutionRepository.CountAsync(s => s.Name == input.Name && s.Id != input.Id) == 0;

            return flag;
        }

        /// <summary>
        /// 批量排班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CreateMultiMachineShift(MultiMachineShiftInputDto input)
        {
            if (!input.MachineIdList.Any())
            {
                return null;
            }

            var shiftedMahcineIdList = await this.ListShiftedMachineId(input);

            var unShiftedMachineIdList = input.MachineIdList.Where(s => !shiftedMahcineIdList.Contains(s)).ToArray();

            for (int i = 0; i < unShiftedMachineIdList.Count(); i++)
            {
                await this.UpdateMachineShiftSolution(new Machine4ShiftSolutionInputDto()
                {
                    Id = unShiftedMachineIdList[i],
                    Machine4ShiftSolutionInputDetail = new List<Machine4ShiftSolutionInputDetail>()
                    {
                        new Machine4ShiftSolutionInputDetail
                        {
                            Id=await this.GetMachineShiftSolutionId(new Machine4ShiftSolutionInputDetail()
                            {
                                ShiftSolutionId=input.ShiftSolutionId,StartTime=input.StartTime,EndTime=input.EndTime
                            }),
                            ShiftSolutionId=input.ShiftSolutionId,
                            StartTime=input.StartTime,
                            EndTime=input.EndTime
                        }
                    }
                });
            }

            if (!shiftedMahcineIdList.Any())
            {
                return null;
            }

            var machines = string.Join(",", this.machineRepository.GetAll().Where(s => shiftedMahcineIdList.Contains(s.Id)).Select(s => s.Code).ToArray());

            var message = this.L("SchedulingHasBeenMadeDuringTime{0}{1}{2}", machines, input.StartTime, input.EndTime);

            return message;
        }

        [UnitOfWork(false)]
        public async Task BatchDeleteMachineShift(MultiMachineShiftInputDto input)
        {
            await shiftRepository.BatchDeleteMachineShift(input.MachineIdList);
        }

        private async Task<int> GetMachineShiftSolutionId(Machine4ShiftSolutionInputDetail input)
        {
            var shiftedMahcineIdList = await (from ms in this.machineShiftEffectiveIntervalRepository.GetAll()
                                              where ms.MachineId == input.Id && (ms.StartTime <= input.EndTime && ms.EndTime >= input.StartTime)
                                              select ms.Id).FirstOrDefaultAsync();

            return shiftedMahcineIdList;
        }

        public async Task<List<int>> ListShiftedMachineId(MultiMachineShiftInputDto input)
        {
            var shiftedMahcineIdList = await (from ms in this.machineShiftEffectiveIntervalRepository.GetAll()
                                              where input.MachineIdList.Contains(ms.MachineId) && (ms.StartTime <= input.EndTime && ms.EndTime >= input.StartTime)
                                              select ms.MachineId).ToListAsync();

            return shiftedMahcineIdList;
        }

        private static class Sql
        {
            public static readonly string CheckIfCurrentDayShiftIsWorkingSql = @"
                    SELECT msei.Id
FROM   MachineShiftEffectiveIntervals  AS msei
       JOIN ShiftSolutionItems        AS si
            ON  msei.ShiftSolutionId = si.ShiftSolutionId
WHERE  msei.MachineId = @MachineId
       AND msei.Id = @Id
       AND GETDATE() >= CONVERT(NVARCHAR(10), msei.StartTime, 23) + ' ' + 
           CONVERT(VARCHAR(8), si.StartTime, 108)
                    ";

            public static string GetMachineShiftSolutionsSql(string whereClause)
            {
                return
                    $@"
SELECT A.Id,
    B.MachineGroupName,
    A.MachineId,
    A.MachineName,
    A.ShiftSolutionId,
    A.ShiftSolutionName,
    A.CreationTime,
    A.StartTime,
    A.EndTime
FROM   (
           SELECT CASE 
                       WHEN sm.isdeleted = 1 THEN 0
                       ELSE sm.Id
                  END        Id,
                  m.Id       MachineId,
                  m.Name     MachineName,
                  m.SortSeq  MachineSortSeq,
				  m.Code     MachineCode,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN 0
                       ELSE ISNULL(sm.ShiftSolutionId, 0)
                  END        ShiftSolutionId,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN 'NotAssociated'
                       ELSE ISNULL(ss.Name, 'NotAssociated')
                  END ShiftSolutionName,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN NULL
                       ELSE CONVERT(CHAR(10), ISNULL(sm.CreationTime, NULL), 23)
                  END CreationTime,
                  StartTime,
                  EndTime
           FROM   DeviceGroups AS dg
                  INNER JOIN MachineDeviceGroups AS mdg ON dg.Id = mdg.DeviceGroupId
                  INNER JOIN Machines AS m  ON  mdg.MachineId = m.Id
                  LEFT JOIN MachineShiftEffectiveIntervals AS sm
                       ON  m.Id = sm.MachineId
                       AND sm.EndTime>=convert(date,GETDATE()) 
                       AND sm.IsDeleted = 0
                  LEFT JOIN ShiftSolutions AS ss
                       ON  sm.ShiftSolutionId = ss.Id
           { whereClause }
       ) AS A
       JOIN (
                SELECT m2.Id MachineId,
                       MachineGroupName = STUFF(
                           (
                               SELECT ',' + dg.DisplayName
                               FROM   Machines AS m
                                      INNER JOIN MachineDeviceGroups AS mdg
                                           ON  mdg.MachineId = m.Id
                                      JOIN DeviceGroups AS dg
                                           ON  dg.Id = mdg.DeviceGroupId
                               WHERE  m.Id = m2.Id AND dg.IsDeleted = 0 
                                      FOR XML PATH('')
                           ),
                           1,
                           1,
                           ''
                       )
                FROM   Machines AS m2
            ) B
            ON  a.MachineId = b.MachineId
GROUP BY
       A.Id,
       B.MachineGroupName,
       A.MachineId,
       A.MachineName,
       a.ShiftSolutionId,
       a.ShiftSolutionName,
       a.CreationTime,
       a.StartTime,
       a.EndTime,
	   A.MachineSortSeq,
	   A.MachineCode
ORDER BY 
       A.MachineSortSeq,
       A.MachineCode,
       a.MachineId
";
            }
        }
    }
}