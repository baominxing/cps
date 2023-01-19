using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto;
using Wimi.BtlCore.Dto;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.BasicData.Machines.Repository;
using WIMI.BTL.Machines.RepositoryDto.State;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.DeviceGroups.Dto;
using Wimi.BtlCore.Archives;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap
{
    [AbpAuthorize(PermissionNames.Pages_StatusDistributionMap)]
    public class StatusDistributionMapAppService : BtlCoreAppServiceBase, IStatusDistributionMapAppService
    {
        private readonly IMachineManager machineManager;
        private readonly IRepository<State, long> stateRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly IRepository<MachinesShiftDetail> machineShiftDetailRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IStateRepository statesRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;

        public StatusDistributionMapAppService(IMachineManager machineManager,
            IRepository<State, long> stateRepository,
            IRepository<StateInfo> stateInfoRepository,
            IRepository<MachinesShiftDetail> machineShiftDetailRepository,
            IRepository<Machine> machineRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            IStateRepository statesRepository,
            DeviceGroupManager deviceGroupManager,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.machineManager = machineManager;
            this.stateRepository = stateRepository;
            this.stateInfoRepository = stateInfoRepository;
            this.machineShiftDetailRepository = machineShiftDetailRepository;
            this.machineRepository = machineRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.statesRepository = statesRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.archiveEntryRepository = archiveEntryRepository;
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListMachines(DeviceGroupDto input)
        {
            if (input.Id != 0)
            {
                var deviceGroupCode = deviceGroupManager.GetCode(input.Id);
                return this.machineManager.ListMachinesInDeviceGroup(deviceGroupCode).Select(p => new NameValueDto<int>() { Name = p.Name, Value = p.Id });
            }
            else
            {
                return await machineManager.ListOrderedMachines();
            }

        }

        public async Task<IEnumerable<NameValueDto<int>>> ListDeviceGroups()
        {
            var deviceGroupList = await deviceGroupManager.ListFirstClassDeviceGroups();
            return deviceGroupList.Select(p => new NameValueDto<int>() { Name = p.DisplayName, Value = p.Id });
        }

        [HttpPost]
        public async Task UpdateMemo(StatusDistributionItemDto input)
        {
            var state = await this.stateRepository.GetAsync(input.StateId);
            state.Memo = input.Memo;
            await this.stateRepository.UpdateAsync(state);
        }

        public async Task<DatatablesPagedResultOutput<StatusDistributionItemDto>> ListStatusByMachine(StatusDistributionDetailRequireDto input)
        {
            var endTime = input.ShiftDay.AddDays(1);

            input.UnionTables = this.GetUnionTables(input.ShiftDay, endTime);

            var query = await this.statesRepository.ListMahcineStateMaps(input.ShiftDay, endTime, new List<long> { input.MachineId }, input.UnionTables);

            #region 区分反馈原因与状态
            var temp = new List<ListMahcineStateMapDto>();
            var machines = await this.machineRepository.GetAllListAsync();
            var stateInfos = await this.stateInfoRepository.GetAllListAsync();
            foreach (var s in query.Where(s => s.STATETYPE == "NONE"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "OUT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.StartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "LEFT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.StartTime, EndTime = s.REndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "LEFT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.REndTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "RIGHT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.RStartTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "RIGHT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.RStartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.RStartTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.RStartTime, EndTime = s.REndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.REndTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            #endregion

            var tempQuery = from t in temp
                            join si in stateInfos on t.StateCode equals si.Code
                            join m in machines on t.MachineId equals m.Id
                            select new StatusDistributionItemDto()
                            {
                                StartTime = t.StartTime,
                                EndTime = t.EndTime,
                                StateName = si.DisplayName,
                                Hexcode = si.Hexcode,
                                StateCode = t.StateCode
                            };

            var result = tempQuery.OrderBy(s => s.StartTime).Skip(input.SkipCount).Take(input.Length).ToList();
            var count = tempQuery.Count();

            result.Where(r => r.EndTime.Date > r.StartTime.Date).ToList().ForEach(r =>
            {
                r.EndTime = Convert.ToDateTime($"{r.StartTime.Date:yyyy-MM-dd} 23:59:59");
            });

            return new DatatablesPagedResultOutput<StatusDistributionItemDto>(count, result, count);
        }

        public async Task<DatatablesPagedResultOutput<StatusDistributionItemDto>> ListStatusByShiftDetail(StatusDistributionDetailRequireDto input)
        {
            var machineShiftDetail = this.machineShiftDetailRepository.Get((int)input.MachineShiftDetailId);
            input.UnionTables = this.GetUnionTables(machineShiftDetail.ShiftDay, machineShiftDetail.ShiftDay);
            var query = (await this.statesRepository.ListMahcineStateMapsByShift(machineShiftDetail.ShiftDay, input.MachineId, input.UnionTables))
                .Where(
                    s => s.MachinesShiftDetailId == input.MachineShiftDetailId)
                .Select(m => new StatusDistributionItemDto
                {
                    StateName = m.StateName,
                    StartTime = m.StartTime,
                    EndTime = m.EndTime
                });
            var result = query.OrderBy(t => t.StartTime)
                              .Skip(input.SkipCount)
                              .Take(input.Length)
                              .ToList();
            var count = query.Count();

            return new DatatablesPagedResultOutput<StatusDistributionItemDto>(count, result, count);
        }
        /// <summary>
        /// 设备当天汇总信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static IEnumerable<DailyStatusSummaryDto> ListStatusSummary(IEnumerable<ListMahcineStateMapDto> input)
        {
            input.Where(r => r.EndTime.Date > r.StartTime.Date).ForEach(r =>
            {
                r.EndTime = Convert.ToDateTime($"{r.StartTime.Date:yyyy-MM-dd} 23:59:59");
            });

            var result = input.GroupBy(n => new { n.MachineId, n.StateName, n.StateCode, n.Hexcode }, (key, g) => new
            {
                key.MachineId,
                key.Hexcode,
                key.StateName,
                key.StateCode,
                Duration = g.Sum(t => (t.EndTime - t.StartTime).TotalHours)
            }).ToList();

            var totalDuration = result.Sum(t => t.Duration);
            return result.Select(t => new DailyStatusSummaryDto
            {
                Hexcode = t.Hexcode,
                Hour = Math.Round(t.Duration, 2),
                StateName = t.StateName,
                StateCode = t.StateCode,
                Percent = totalDuration <= 0 ? 0 : Math.Round((t.Duration / totalDuration * 1.0) * 100, 2)
            });
        }

        /// <summary>
        /// 设备当天汇总信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DailyStatusSummaryDto>> ListStatusSummary(DailyStatusSummaryRequireDto input)
        {
            var endTime = input.ShiftDay.AddDays(1);
            var stateQueryable = this.stateRepository.GetAll().Where(s => s.MachineId == input.MachineId);
            stateQueryable = input.MachinesShiftDetailId.HasValue
                ? stateQueryable.Where(s => s.MachinesShiftDetailId == input.MachinesShiftDetailId)
                : stateQueryable.Where(s => s.StartTime >= input.ShiftDay && s.StartTime < endTime);

            var query = await (from s in stateQueryable
                               join si in this.stateInfoRepository.GetAll() on s.Code equals si.Code
                               select new DailyStatusSummaryItemDto
                               {
                                   MachineId = s.MachineId,
                                   StateCode = s.Code,
                                   StateName = si.DisplayName,
                                   Hexcode = si.Hexcode,
                                   StartTime = s.StartTime ?? DateTime.Now,
                                   EndTime = s.EndTime ?? DateTime.Now
                               }).ToListAsync();

            query.Where(r => r.EndTime.Date > r.StartTime.Date).ForEach(r =>
            {
                r.EndTime = Convert.ToDateTime($"{r.StartTime.Date:yyyy-MM-dd} 23:59:59");
            });

            var result = query.GroupBy(n => new { n.MachineId, n.StateName, n.StateCode, n.Hexcode }, (key, g) => new
            {
                key.MachineId,
                key.Hexcode,
                key.StateName,
                key.StateCode,
                Duration = g.Sum(t => (t.EndTime - t.StartTime).TotalHours)
            }).ToList();

            var totalDuration = result.Sum(t => t.Duration);
            return result.Select(t => new DailyStatusSummaryDto
            {
                Hexcode = t.Hexcode,
                Hour = Math.Round(t.Duration, 2),
                StateName = t.StateName,
                StateCode = t.StateCode,
                Percent = totalDuration <= 0 ? 0 : Math.Round((t.Duration / totalDuration * 1.0) * 100, 2)
            });
        }


        /// <summary>
        /// 页面主数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StatusDistributionDto>> ListStatusDistribution(StatusDistributionRequireDto input)
        {
            var startTime = DateTime.ParseExact(input.StartTime.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture);
            var endTime = DateTime.ParseExact(input.EndTime.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture).AddDays(1);

            input.UnionTables = this.GetUnionTables(startTime, endTime);

            var query = await this.statesRepository.ListMahcineStateMaps(startTime, endTime, input.MachineIdList, input.UnionTables);

            #region 区分反馈原因与状态
            var temp = new List<ListMahcineStateMapDto>();
            var machines = await this.machineRepository.GetAllListAsync();
            var stateInfos = await this.stateInfoRepository.GetAllListAsync();

            foreach (var s in query.Where(s => s.STATETYPE == "NONE"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "OUT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.StartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "LEFT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.StartTime, EndTime = s.REndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "LEFT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.REndTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "RIGHT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.RStartTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "RIGHT"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.RStartTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.StartTime, EndTime = s.RStartTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.StateCode, StartTime = s.RStartTime, EndTime = s.REndTime, DateKey = s.DateKey });
            };

            foreach (var s in query.Where(s => s.STATETYPE == "IN"))
            {
                temp.Add(new ListMahcineStateMapDto() { MachineId = s.MachineId, StateCode = s.Code, StartTime = s.REndTime, EndTime = s.EndTime, DateKey = s.DateKey });
            };

            #endregion

            var tempQuery = from t in temp
                            join si in stateInfos on t.StateCode equals si.Code
                            join m in machines on t.MachineId equals m.Id
                            select new ListMahcineStateMapDto()
                            {
                                MachineId = m.Id,
                                MachineName = m.Name,
                                MachineSeq = m.SortSeq,
                                DateKey = t.DateKey,
                                StartTime = t.StartTime,
                                EndTime = t.EndTime,
                                StateName = si.DisplayName,
                                Hexcode = si.Hexcode,
                                StateCode = t.Code
                            };

            var groupBy = tempQuery.GroupBy(n => new { n.MachineName, n.MachineId, n.DateKey, n.MachineSeq }).OrderBy(g => g.Key.MachineSeq);
            var result = new List<StatusDistributionDto>();
            foreach (var item in groupBy)
            {
                var shiftDay = DateTime.ParseExact(item.Key.DateKey.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture);
                var rate = ListStatusSummary(tempQuery.Where(q => q.MachineId == item.Key.MachineId));
                var value = new StatusDistributionDto
                {
                    MachineId = item.Key.MachineId,
                    MachineName = item.Key.MachineName,
                    ShiftDay = shiftDay,
                    Data = item.OrderBy(t => t.StartTime).Select(t =>
                    {
                        var endDateTime = t.EndTime.Date > t.StartTime.Date
                                ? Convert.ToDateTime($"{t.StartTime.Date:yyyy-MM-dd} 23:59:59")
                                : t.EndTime;
                        return new StatusDistributionItemDto
                        {
                            StateName = t.StateName,
                            StartTime = t.StartTime,
                            EndTime = endDateTime,
                            Hexcode = t.Hexcode
                        };
                    }),
                    StatusSummaryRate = rate
                };

                result.Add(value);
            }

            return result;
        }

        // 班次详细数据
        public async Task<IEnumerable<ShiftStatusDistributionDto>> ListShiftStatusDistribution(DailyStatusSummaryRequireDto input)
        {
            input.UnionTables = this.GetUnionTables(input.ShiftDay, input.ShiftDay);
            var query = await this.statesRepository.ListMahcineStateMapsByShift(input.ShiftDay, input.MachineId, input.UnionTables);
            var shiftDateTimeRange = (await shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange(input.MachineId, input.ShiftDay)).ToList();
            if (!shiftDateTimeRange.Any())
            {
                throw new UserFriendlyException(this.L("EquipmentNotScheduledTip"));
            }

            var groupBy = query.GroupBy(n => new { n.MachinesShiftDetailId, n.ShiftName, n.ShiftSolutionItemId }).OrderBy(n => n.Key.ShiftSolutionItemId);

            var resut = new List<ShiftStatusDistributionDto>();
            foreach (var item in groupBy)
            {
                var range = shiftDateTimeRange.First(s => s.MachineShiftDetailId == item.Key.MachinesShiftDetailId);
                var rate = ListStatusSummary(query.Where(q => q.MachinesShiftDetailId == item.Key.MachinesShiftDetailId)).ToList();

                var value = new ShiftStatusDistributionDto
                {
                    ShiftName = item.Key.ShiftName,
                    MachineId = input.MachineId,
                    StartTime = range.BeginTime,
                    EndTime = range.FinishTime,
                    StatusSummaryRate = rate,
                    MachineShiftDetailId = item.Key.MachinesShiftDetailId,
                    Data = item.ToList().OrderBy(t => t.StartTime).Select(t => new StatusDistributionItemDto
                    {
                        StateName = t.StateName,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        Hexcode = t.Hexcode
                    })
                };
                resut.Add(value);
            }

            return resut;
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListSummaryConditions(StatusDistributionRequireDto input)
        {
            switch (input.Mode)
            {
                case EnumStatisticalMode.ByMachine:
                    var machines = (await this.machineManager.ListOrderedMachines()).Where(n => input.MachineIdList.Contains(n.Value));
                    return machines;
                case EnumStatisticalMode.ByShiftday:
                    var summaryData =
                        await this.shiftDayTimeRangeRepository.ListSummaryDataRange(input.StartTime, input.EndTime);
                    return summaryData;
                default:
                    throw new Exception("");
            }
        }

        [HttpPost]
        public async Task<StatusDistributionItemDto> GetStateInfo(EntityDto input)
        {
            var state = await this.stateRepository.GetAll()
                .Join(this.stateInfoRepository.GetAll(), s => s.Code, info => info.Code,
                    (s, info) => new { state = s, stateInfo = info }).Where(s => s.state.Id == input.Id)
                .Select(s =>
                    new StatusDistributionItemDto
                    {
                        StateId = s.state.Id,
                        StateName = s.stateInfo.DisplayName,
                        StateCode = s.stateInfo.Code,
                        Memo = s.state.Memo,
                        StartTime = s.state.StartTime ?? DateTime.Now,
                        EndTime = s.state.EndTime ?? DateTime.Now,
                        Hexcode = s.stateInfo.Hexcode
                    }).FirstOrDefaultAsync();

            return state;
        }
        private List<string> GetUnionTables(DateTime startTime, DateTime endTime)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => startTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= endTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
    }
}