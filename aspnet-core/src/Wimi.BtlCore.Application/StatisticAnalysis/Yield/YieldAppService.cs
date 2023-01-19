namespace Wimi.BtlCore.StatisticAnalysis.Yield
{
    using Abp.Collections.Extensions;
    using Abp.Domain.Repositories;
    using Castle.Components.DictionaryAdapter;
    using Common;
    using CommonEnums;
    using Configuration;
    using Dapper;
    using Dto;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Archives;
    using Wimi.BtlCore.BasicData.Capacities;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
    using Wimi.BtlCore.BasicData.Shifts.Manager;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.StatisticAnalysis.Yield.Export;
    using Wimi.BtlCore.ThirdpartyApis.Dto;

    public class YieldAppService : BtlCoreAppServiceBase, IYieldAppService
    {
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IShiftCalendarManager shiftCalendarManager;
        private readonly MachineManager machineManager;
        private readonly IYieldExporter yieldExporter;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly ICapacityRepository capacityRepository;
        private readonly IStateRepository stateRepository;

        public YieldAppService(ICommonLookupAppService commonLookupAppService,
            IShiftCalendarManager shiftCalendarManager,
            MachineManager machineManager,
            IYieldExporter yieldExporter,
            IRepository<ArchiveEntry> archiveEntryRepository,
            ICapacityRepository capacityRepository,
            IStateRepository stateRepository)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.shiftCalendarManager = shiftCalendarManager;
            this.machineManager = machineManager;
            this.yieldExporter = yieldExporter;
            this.archiveEntryRepository = archiveEntryRepository;
            this.capacityRepository = capacityRepository;
            this.stateRepository = stateRepository;
        }

        [HttpPost]
        public GetMachineStateRateInputDto GetStartTimeOfGanttChart(GetMachineStateRateInputDto input)
        {
            //获取甘特图得开始结束时间 获取当前工厂日得开始结束时间 这里注意获取得是所有启用设备里面最大得班次方案里面得开始结束时间
            //根据工厂日得时间
            var returnValue = new GetMachineStateRateInputDto()
            {
                StartTime = Convert.ToDateTime(input.SummaryDate).AddHours(Convert.ToDouble(SettingManager.GetSettingValue(AppSettings.Visual.StartHourInGantChart))).AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = Convert.ToDateTime(input.SummaryDate).AddHours(Convert.ToDouble(SettingManager.GetSettingValue(AppSettings.Visual.StartHourInGantChart))).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")
            };

            returnValue = capacityRepository.GetStartTimeOfGanttChart(input);
            return returnValue;
        }

        /// <summary>
        ///获取第一次查询时的参数，默认前台页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<GetMachineYieldQueryParamDto> GetFirstQueryParam(QueryDateTimeDto input)
        {
            var catchMachines = await commonLookupAppService.GetDeviceGroupAndDefaultCountMachineWithPermissions();

            var endTime = DateTime.Today.ToString("yyyy-MM-dd");
            var startTime = DateTime.Today.AddDays(-6).ToString("yyyy-MM-dd");

            if (input.StartTime.HasValue)
            {
                startTime = input.StartTime.Value.ToString("yyyy-MM-dd");
            }

            if (input.EndTime.HasValue)
            {
                endTime = input.EndTime.Value.ToString("yyyy-MM-dd");
            }

            var summaryDataList = await GetSummaryDate(new GetMachineStateRateInputDto
            {
                EndTime = endTime,
                StartTime = startTime
            });

            return new GetMachineYieldQueryParamDto
            {
                MachineIdList = catchMachines.Machines.Select(m => m.Id).ToList(),
                DataList = summaryDataList.Select(s => s.SummaryDate).OrderByDescending(s => s).ToList()
            };
        }

        [HttpPost]
        public async Task<IEnumerable<Yield4PerProgramOutputDto>> GetMachineAvgProgramDurationAndYield(MachineDetailYieldInfoInputDto input)
        {
            var date = GetStartTimeOfGanttChart(new GetMachineStateRateInputDto
            {
                SummaryDate = input.SummaryDate
            });

            return await capacityRepository.GetMachineAvgProgramDurationAndYield(input.MachineId.Value, date.StartTime, date.EndTime);
        }

        [HttpPost]
        public async Task<IEnumerable<GetMachineGanttChartOutputDto>> GetMachineDetailGanttCharat(MachineDetailYieldInfoInputDto input)
        {
            if (input.MachineId.HasValue) input.MachineIdList = new List<int> { input.MachineId.Value };
            var summaryDayData = await GetMachineStatesGanttChart(input);
            var date = GetStartTimeOfGanttChart(new GetMachineStateRateInputDto
            {
                SummaryDate = input.SummaryDate
            });
            var timeRangeList = new[]
                                    {
                                        new Tuple<DateTime, DateTime>(
                                            Convert.ToDateTime(date.StartTime),
                                            Convert.ToDateTime(date.StartTime).AddHours(8)),
                                        new Tuple<DateTime, DateTime>(
                                            Convert.ToDateTime(date.StartTime).AddHours(8),
                                            Convert.ToDateTime(date.StartTime).AddHours(16)),
                                        new Tuple<DateTime, DateTime>(
                                            Convert.ToDateTime(date.StartTime).AddHours(16),
                                            Convert.ToDateTime(date.StartTime).AddHours(24))
                                    };

            var getMachineGanttChartOutputDtos = summaryDayData as GetMachineGanttChartOutputDto[] ?? summaryDayData.ToArray();
            if (getMachineGanttChartOutputDtos.Any())
                foreach (var item in getMachineGanttChartOutputDtos)
                {
                    var returnValue =
                        new GanttChartOutputDto
                        {
                            ShowXAxis = true,
                            Name = item.ChartDataList.Name,
                            Id = item.MachineId
                        };

                    foreach (var interval in item.ChartDataList.Intervals)
                    {
                        var intervalList = new List<Intervals>();
                        foreach (var tuple in timeRangeList)
                        {
                            var newInterval = new Intervals
                            {
                                Datetime =
                                                          new[]
                                                              {
                                                                  tuple.Item1.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                  tuple.Item2.ToString("yyyy-MM-dd HH:mm:ss")
                                                              },
                                State = interval.State.Where(
                                                              t =>
                                                              {
                                                                  var startTime = Convert.ToDateTime(
                                                                      t.StartDatetime);
                                                                  return startTime >= tuple.Item1 && startTime
                                                                         < tuple.Item2;
                                                              })
                                                          .OrderBy(s => s.StartDatetime)
                                                          .ToList(),
                                Reason = interval.Reason.Where(
                                                              t =>
                                                              {
                                                                  var startTime = Convert.ToDateTime(
                                                                      t.StartDatetime);
                                                                  return startTime >= tuple.Item1 && startTime
                                                                         < tuple.Item2;
                                                              })
                                                          .OrderBy(s => s.StartDatetime)
                                                          .ToList()
                            };
                            intervalList.Add(newInterval);
                        }

                        // 处理Reason和State最后一笔结束时间超出范围的问题 例如最后一笔为7:58— 8:02,则要该笔拆分为7:58— 8:00 和8:00-8:02
                        for (var i = 0; i < intervalList.Count; i++)
                        {
                            var key = intervalList[i];
                            var endTime = Convert.ToDateTime(key.Datetime.LastOrDefault());
                            var lastReason = key.Reason.LastOrDefault();
                            var lastState = key.State.LastOrDefault();

                            if (lastState != null && Convert.ToDateTime(lastState.EndDatetime) > endTime)
                            {
                                if (i < intervalList.Count - 1)
                                    intervalList[i + 1]
                                        .State.Insert(
                                            0,
                                            new DataParticles
                                            {
                                                DisplayName = lastState.DisplayName,
                                                Color = lastState.Color,
                                                Message = lastState.Message,
                                                StartDatetime = key.Datetime.Last(),
                                                EndDatetime = lastState.EndDatetime
                                            });
                                key.State[key.State.Count - 1].EndDatetime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }

                            if (lastReason != null && Convert.ToDateTime(lastReason.EndDatetime) > endTime)
                            {
                                if (i < intervalList.Count - 1)
                                    intervalList[i + 1]
                                        .State.Insert(
                                            0,
                                            new DataParticles
                                            {
                                                DisplayName = lastReason.DisplayName,
                                                Color = lastReason.Color,
                                                Message = lastReason.Message,
                                                StartDatetime = key.Datetime.Last(),
                                                EndDatetime = lastReason.EndDatetime
                                            });
                                key.Reason[key.Reason.Count - 1].EndDatetime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }

                        returnValue.Intervals = intervalList.ToArray();
                    }

                    item.ChartDataList = returnValue;
                }

            return getMachineGanttChartOutputDtos;
        }

        [HttpPost]
        public async Task<IEnumerable<MachineStateRateOutputDto>> GetMachineStateRate(MachineDetailYieldInfoInputDto input)
        {
            var returnValue = new List<MachineStateRateOutputDto>();

            // 获取正确的所选日期
            var correctedQueryDateList = (await CorrectQueryDate(new GetMachineStateRateInputDto()
            {
                StartTime = input.StartTime?.ToString("yyyy-MM-dd"),
                EndTime = input.EndTime?.ToString("yyyy-MM-dd"),
                MachineIdList = new EditableList<int> { input.MachineId.Value }
            })).ToList();

            if (!correctedQueryDateList.Any())
            {
                return new List<MachineStateRateOutputDto>();
            }

            var data = await stateRepository.GetMachineStateRate(input.MachineId, correctedQueryDateList);

            if (data.Any())
            {
                returnValue = data.ToList();
            }

            return returnValue;
        }
 
        /// <summary>
        ///     获取设备甘特图数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<GetMachineGanttChartOutputDto>> GetMachineStatesGanttChart(MachineDetailYieldInfoInputDto input)
        {
            IEnumerable<GetMachineGanttChartOutputDto> returnValue;

            // 获取正确的所选日期
            var correctedQueryDateList = (await CorrectQueryDate(new GetMachineStateRateInputDto()
            {
                StartTime = input.StartTime?.ToString("yyyy-MM-dd"),
                EndTime = input.EndTime?.ToString("yyyy-MM-dd"),
                MachineIdList = input.MachineIdList
            })).ToList();

            if (!correctedQueryDateList.Any())
            {
                return new List<GetMachineGanttChartOutputDto>();
            }

            var outputList = new List<GetMachineGanttChartOutputDto>();

            foreach (var date in correctedQueryDateList)
            {
                input.StartTime = Convert.ToDateTime(date.StartTime);
                input.EndTime = Convert.ToDateTime(date.EndTime);
                outputList.AddRange(await GetSingleMachineStatesGanttChartDataAsync(input));
            }

            returnValue = outputList;

            return returnValue;
        }

        [HttpPost]
        public async Task<IEnumerable<GetMachineGanttChartOutputDto>> GetSingleMachineStatesGanttChartDataAsync(MachineDetailYieldInfoInputDto input)
        {
            IEnumerable<GetMachineGanttChartOutputDto> returnValue = new List<GetMachineGanttChartOutputDto>();

            if (!input.MachineIdList.Any())
            {
                return returnValue;
            }

            var result = await stateRepository.GetOriginalState(input.MachineIdList, input.StartTime, input.EndTime);
            var reasons = await stateRepository.GetOriginalReasonState(input.MachineIdList, input.StartTime, input.EndTime);

            if (!result.Any())
            {
                return new List<GetMachineGanttChartOutputDto>();
            }

            // 按设备分组
            // ReSharper disable once PossibleMultipleEnumeration
            var groupByResult
                = result
                    .GroupBy(
                        g => g.Id,
                        (key, list) => new GetMachineGanttChartOutputDto
                        {
                            MachineId = key,
                            MachineStatesList = list.ToList()
                        });

            returnValue = groupByResult.Select(
                    n => new GetMachineGanttChartOutputDto
                    {
                        MachineId = n.MachineId,
                        ChartDataList
                            = n.MachineStatesList.Select(
                                t => new GanttChartOutputDto
                                {
                                    Id = t.Id,
                                    Name = t.Name,
                                    Intervals = GetMachineGanttChartIntervals(n, reasons, input.SummaryDate)
                                })
                            .FirstOrDefault()
                    })
                .ToArray();


            return returnValue;
        }

        /// <summary>
        /// 获取一台或多台设备的班次利用率
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// </returns>
        [HttpPost]
        public async Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate(MachineDetailYieldInfoInputDto input)
        {
            var returnValue = new List<UtilizationRateOutputDto>();

            if (input.MachineId.HasValue)
            {
                input.MachineIdList = new EditableList<int> { input.MachineId.Value };
            }

            // 获取正确的所选日期
            var correctedQueryDateList = (await CorrectQueryDate(
                new GetMachineStateRateInputDto()
                {
                    StartTime = input.StartTime?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    EndTime = input.EndTime?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    MachineIdList = input.MachineIdList
                })).ToList();

            var first = correctedQueryDateList.FirstOrDefault();
            var last = correctedQueryDateList.LastOrDefault();

            var startTime = first == null ? DateTime.Now : first.StartTime;
            var endTime = last == null ? DateTime.Now : last.EndTime;

           var data =await stateRepository.GetMachineUtilizationRate(input.MachineIdList, startTime, endTime, input.MachineId);

            if (data.Any())
            {
                returnValue = data.ToList();
            }

            return returnValue;
        }

        [HttpPost]
        public async Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate4Popover(MachineDetailYieldInfoInputDto input)
        {
            var returnValue = new List<UtilizationRateOutputDto>();
            var date = Convert.ToDateTime(input.SummaryDate);
            var dateList = new[] { date, date.AddDays(-8), date.AddDays(-31) };

            foreach (var item in dateList)
            {
                var result = (await GetMachineUtilizationRate(
                                 new MachineDetailYieldInfoInputDto
                                 {
                                     MachineId = input.MachineId,
                                     SummaryDate = item.ToString("yyyy-MM-dd"),
                                     CurrentQueryDate = date,
                                     StartTime = item,
                                     EndTime = item == date ? date : date.AddDays(-1),
                                     IsHistoryDay = item != date
                                 }))
                             .ToArray();
                if (result.Any()) returnValue.AddRange(result);
            }

            return returnValue;
        }

        /// <summary>
        ///     获取产量数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineYieldAnalysisOutputDto>> GetMachineYieldAnalysis(GetMachineStateRateInputDto input)
        {
            // 获取正确的所选日期
            var correctedQueryDateList = (await CorrectQueryDate(input)).ToList();

            var first = correctedQueryDateList.FirstOrDefault();
            var last = correctedQueryDateList.LastOrDefault();

            var startTime = first == null ? DateTime.Now : first.StartTime;
            var endTime = last == null ? DateTime.Now : last.EndTime;

            var machines = await stateRepository.GetMachineData(input.MachineIdList);
            var result = await stateRepository.GetMachineYieldData(input.MachineIdList, startTime, endTime);

            var returnValue = new List<MachineYieldAnalysisOutputDto>();

            // 处理未添加设备
            var machineYieldAnalysisOutputDtos = machines as MachineYieldAnalysisOutputDto[] ?? machines.ToArray();

            foreach (var item in machineYieldAnalysisOutputDtos)
            {
                if (result.All(r => r.MachineId != item.MachineId))
                    result.AddRange(
                        machineYieldAnalysisOutputDtos.Where(m => m.MachineId == item.MachineId)
                            .Select(
                                r => new MachineYieldAnalysisOutputDto
                                {
                                    MachineId = r.MachineId,
                                    MachineGroupId = r.MachineGroupId,
                                    MachineGroupName = r.MachineGroupName,
                                    MachineName = r.MachineName
                                }));
            }
            // 处理分组，按照设备显示，如果设备在多个组中，用逗号分割
            var group = result.GroupBy(m => m.MachineId, (key, g) => new { GroupKey = key, List = g.ToList() });

            foreach (var item in group)
            {
                var groupName = item.List.Select(g => g.MachineGroupName).ToArray().JoinAsString(",");
                item.List.First().MachineGroupName = groupName.Trim(',');
                returnValue.Add(item.List.First());
            }

            // 处理产量趋势
            var yesterdayRate = await GetMachineUtilizationRate(new MachineDetailYieldInfoInputDto
            {
                MachineIdList = input.MachineIdList,
                SummaryDate =
                                                Convert.ToDateTime(input.SummaryDate)
                                                    .AddDays(-1)
                                                    .ToString("yyyy-MM-dd"),
                CurrentQueryDate = Convert.ToDateTime(
                                                input.SummaryDate),
                StartTime = Convert.ToDateTime(input.SummaryDate).AddDays(-1),
                EndTime = Convert.ToDateTime(input.SummaryDate),
            });

            returnValue.ForEach(
                r =>
                {
                    var utilizationRateOutputDto = yesterdayRate.FirstOrDefault(y => y.MachineId == r.MachineId);

                    if (utilizationRateOutputDto != null)
                    {
                        var rate = utilizationRateOutputDto.UtilizationRate;
                        if (r.UtilizationRate.ToString(CultureInfo.InvariantCulture).Equals(rate))
                            r.UtilizationTendency = UtilizationTendencyType.Equal.ToString();
                        else
                            r.UtilizationTendency =
                                r.UtilizationRate > Convert.ToDecimal(rate)
                                    ? UtilizationTendencyType.More.ToString()
                                    : UtilizationTendencyType.Less.ToString();
                    }
                    else if (r.UtilizationRate > 0)
                    {
                        r.UtilizationTendency = UtilizationTendencyType.More.ToString();
                    }
                });

            return returnValue;

        }

        /// <summary>
        ///     根据日历表获取所选日期的时间范围
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineStateDto>> GetSummaryDate(GetMachineStateRateInputDto input)
        {
            return await GetSummaryDate(input);
        }

        private Intervals[] GetMachineGanttChartIntervals(GetMachineGanttChartOutputDto groupData, IEnumerable<GetOriginalState> reasons, string targetDate)
        {
            var date = GetStartTimeOfGanttChart(new GetMachineStateRateInputDto { SummaryDate = targetDate });

            var intervalsTime = new[] { date.StartTime, date.EndTime };

            // 返回数组是为了构造前端需要的数据结构
            return new[]
            {
                new Intervals
                    {
                        Datetime = intervalsTime,

                        // StateInfos中type=0的为状态
                        State = GetDataParticlesList(groupData.MachineStatesList, 0),
                        Reason = ListReasonDataParticles(groupData,reasons)
                    }
            };
        }

        private List<DataParticles> GetDataParticlesList(List<GetOriginalState> inputList, int type)
        {
            return inputList.Where(m => m.Type == type)
                .Select(
                    m => new DataParticles
                    {
                        Color = m.Color,
                        DisplayName = L(m.Code),
                        EndDatetime = m.EndDatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Message = new { memo = string.Empty },
                        StartDatetime = m.StartDatetime.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                .OrderBy(s => s.StartDatetime)
                .ToList();
        }

        private List<DataParticles> ListReasonDataParticles(GetMachineGanttChartOutputDto groupData, IEnumerable<GetOriginalState> reasons)
        {
            return reasons.Where(r => r.Id == groupData.MachineId).Select(
                    m => new DataParticles
                    {
                        Color = m.Color,
                        DisplayName = m.DisplayName,
                        EndDatetime = m.EndDatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Message = new { memo = string.Empty },
                        StartDatetime = m.StartDatetime.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                .OrderBy(s => s.StartDatetime)
                .ToList();
        }

        /// <summary>
        /// 获取修正过的班次日，周，月和年的日期
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftCalendarDto>> CorrectQueryDate(GetMachineStateRateInputDto input)
        {
            var startDate = input.StartTime == null ? DateTime.Today:  Convert.ToDateTime(input.StartTime);
            var endDate = input.StartTime == null ? DateTime.Today : Convert.ToDateTime(input.EndTime);
            input.MachineIdList = !input.MachineIdList.Any()
                ? (await this.machineManager.ListMachines()).Select(t => t.Value).ToList()
                : input.MachineIdList.Distinct().ToList();

            var query = await shiftCalendarManager.CorrectQueryDate(startDate, endDate, input.StatisticalWay,
                input.MachineIdList, input.ShiftSolutionIdList);

            return ObjectMapper.Map<IEnumerable<ShiftCalendarDto>>(query);
        }

        [HttpPost]
        public async Task<MachineYieldDto> GetMachineCapability(GetMachineYieldInputDto input)
        {
            var startDate = Convert.ToDateTime(input.StartTime);
            var endDate = Convert.ToDateTime(input.EndTime);

            var returnedValue = new MachineYieldDto();
            input.MachineIdList = !input.MachineIdList.Any()
                ? (await this.machineManager.ListDefaultSearchMachines()).Select(t => t.Value).ToList()
                : input.MachineIdList.Distinct().ToList();

            var dateList = (await shiftCalendarManager.CorrectQueryDate(startDate, endDate, input.StatisticalWay,
                input.MachineIdList, input.ShiftSolutionIdList)).ToList();

            if (!dateList.Any())
            {
                return returnedValue;
            }

            var startTime = dateList.First().ShiftDay;
            var endTime = dateList.Last().ShiftDay;

            var unionTables = this.GetUnionTables(input);

            returnedValue = await capacityRepository.GetMachineCapability(input.StatisticalWay, input.QueryMethod, input.MachineIdList
                , input.ShiftSolutionIdList, input.DeviceGroupIdList, startTime, endTime, unionTables);

            return returnedValue;
        }

        public async Task<FileDto> Export(GetMachineYieldInputDto input)
        {
            var query = await this.GetMachineCapability(input);

            return this.yieldExporter.ExportToFile(query);
        }
        private List<string> GetUnionTables(GetMachineYieldInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "Capacities").ToList()
                .Where(s => Convert.ToDateTime(input.StartTime) <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= Convert.ToDateTime(input.EndTime))
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
    }
}