namespace Wimi.BtlCore.StatisticAnalysis.OEE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.Authorization;
    using Abp.AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.OEE;
    using Wimi.BtlCore.StatisticAnalysis.OEE.Dto;
    using Wimi.BtlCore.StatisticAnalysis.Yield;
    using Wimi.BtlCore.StatisticAnalysis.Yield.Dto;

    [AbpAuthorize(PermissionNames.Pages_OEE)]
    public class OeeAnalysisAppService : BtlCoreAppServiceBase, IOeeAnalysisAppService
    {
        private readonly IOeeRepository oeeRepository;
        private readonly IOeeAnalysisManager oeeAnalysisManager;
        private readonly IYieldAppService yieldAppService;

        public OeeAnalysisAppService(IOeeRepository oeeRepository, IYieldAppService yieldAppService, IOeeAnalysisManager oeeAnalysisManager)
        {
            this.oeeRepository = oeeRepository;
            this.yieldAppService = yieldAppService;
            this.oeeAnalysisManager = oeeAnalysisManager;
        }

        public async Task<OeeDto> ListMachineOEEChart(OeeAnalysis input)
        {
            var paramter = await this.oeeAnalysisManager.FormartInputDto(input);

            if (paramter.StatisticalWays == EnumStatisticalWays.ByShift)
            {
                return await this.ListShiftOee(paramter);
            }

            var availability = await this.ListMachineAvailability(paramter);
            var qualityIndicator = await this.ListQualityIndicators(paramter);
            var performanceIndicator = await this.ListPerformanceIndicators(paramter);
           
            var query = (from av in availability
                         join q in qualityIndicator on new { av.MachineId, av.ShiftDay } equals new { q.MachineId, q.ShiftDay } into g
                         from k in g.DefaultIfEmpty()
                         join mp in performanceIndicator on new { av.MachineId, av.ShiftDay } equals new { mp.MachineId, mp.ShiftDay } into pg
                         from kp in pg.DefaultIfEmpty()
                         select new MachineOEEDto()
                                  {
                                        MachineId = av.MachineId,
                                        MachineName = av.MachineName,
                                        ShiftDay = av.ShiftDay,
                                        Value = kp == null || k == null ? 0 : Math.Round(kp.Rate * av.Rate * k.Rate * 100, 2)
                                    }).ToList();
            return new OeeDto
                       {
                           MachineOee = query.OrderBy(q => q.ShiftDay).ThenBy(q => q.MachineId).ToList(),
                           ShiftDayRanges = paramter.ShiftDayRanges
                       };
        }

        [HttpPost]
        public async Task<OeeDetailDto> GetMachineOEEDetail(OeeAnalysis input)
        {
            var paramter = await this.oeeAnalysisManager.FormartInputDto(input);

            if (paramter.StatisticalWays == EnumStatisticalWays.ByShift)
            {
                return await this.GetMachineShiftOeeDetail(paramter);
            }

            var availability = (await this.ListMachineAvailability(paramter)).FirstOrDefault();
            var qualityIndicator = (await this.ListQualityIndicators(paramter)).FirstOrDefault();
            var performanceIndicator = (await this.ListPerformanceIndicators(paramter)).FirstOrDefault();

            return new OeeDetailDto()
                       {
                           Availability = availability?.Rate.ToString("P"),
                           QualityIndicators = qualityIndicator?.Rate.ToString("P"),
                           PerformanceIndicators = performanceIndicator?.Rate.ToString("P")
                       };
        }

        public async Task<IEnumerable<MachineAvailabilityDto>> ListMachineAvailability(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListMachineAvailability(input);
            var query = result
                .Join(input.ShiftDayRanges, r => r.ShiftDay, s => s.Name, (r, s) => new { ShiftDay = s, Result = r })
                .GroupBy(q => new { q.Result.MachineId, q.Result.MachineName, ShiftDay = q.ShiftDay.Value }).Select(
                    n => new MachineAvailabilityDto()
                             {
                                 ActualWorkTime = n.First().Result.ActualWorkTime,
                                 PlannedWorkTime = n.First().Result.PlanedWorkingTime,
                                 ShiftDay = n.Key.ShiftDay,
                                 MachineId = n.Key.MachineId,
                                 MachineName = n.Key.MachineName
                             });

            return query.ToList();
        }

        public async Task<IEnumerable<QualityStatusDto>> ListQualityIndicators(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListQualityIndicators(input);

            var query = result
                .Join(input.ShiftDayRanges, r => r.ShiftDay, s => s.Name, (r, s) => new { ShiftDay = s, Result = r })
                .GroupBy(q => new { q.Result.MachineId, q.Result.MachineName, ShiftDay = q.ShiftDay.Value }).Select(
                    n => new QualityStatusDto()
                             {
                                 TotalCount = n.Sum(t => t.Result.TotalCount),
                                 UnqualifiedCount = n.Sum(t => t.Result.UnqualifiedCount),
                                 ShiftDay = n.Key.ShiftDay,
                                 MachineId = n.Key.MachineId,
                                 ProductName = n.First().Result.ProductName,
                                 ProductId = n.First().Result.ProductId
                             });

            return query.ToList();
        }

        public async Task<IEnumerable<QualityStatusDto>> ListQualityIndicatorsItemByProduct(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListQualityIndicatorsItemByProduct(input);

            var query = result
                .Join(input.ShiftDayRanges, r => r.ShiftDay, s => s.Name, (r, s) => new { ShiftDay = s, Result = r })
                .GroupBy(q => new { q.Result.MachineId, q.Result.MachineName, ShiftDay = q.ShiftDay.Value }).ToList();

            var retrunValue = new List<QualityStatusDto>();
            query.ForEach(
                t =>
                    {
                        retrunValue.AddRange(
                            t.Select(
                                n => new QualityStatusDto()
                                         {
                                             TotalCount = n.Result.TotalCount,
                                             UnqualifiedCount = n.Result.UnqualifiedCount,
                                             ShiftDay = t.Key.ShiftDay,
                                             MachineId = t.Key.MachineId,
                                             ProductName = n.Result.ProductName,
                                             ProductId = n.Result.ProductId
                                         }));
                    });


            return retrunValue;
        }

        public async Task<IEnumerable<MachinePerformanceIndicator>> ListPerformanceIndicators(OeeAnalysis input)
        {
           var result = await this.oeeRepository.ListPerformanceIndicators(input);

            var query = result
                .Join(input.ShiftDayRanges, r => r.ShiftDay, s => s.Name, (r, s) => new { ShiftDay = s, Result = r })
                .GroupBy(q => new { q.Result.MachineId, q.Result.MachineName, ShiftDay = q.ShiftDay.Value }).ToList();
                
                var returnValue = query.Select(
                    n => new MachinePerformanceIndicator()
                             {
                                 TotalDuration = n.Sum(t => t.Result.TotalDuration),
                                 TotalYiled = n.Sum(t => t.Result.TotalYiled),
                                 PerfectTime = n.Sum(t => t.Result.PerfectTime),
                                 ShiftDay = n.Key.ShiftDay,
                                 MachineId = n.Key.MachineId,
                                 ProductName = n.First().Result.ProductName,
                                 ProductId = n.First().Result.ProductId
                    });

            return returnValue.ToList();
        }


        public async Task<IEnumerable<MachinePerformanceIndicator>> ListPerformanceIndicatorsItemByProduct(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListPerformanceIndicatorsItemByProduct(input);

            var query = result
                .Join(input.ShiftDayRanges, r => r.ShiftDay, s => s.Name, (r, s) => new { ShiftDay = s, Result = r })
                .GroupBy(q => new { q.Result.MachineId, q.Result.MachineName, ShiftDay = q.ShiftDay.Value }).ToList();

            var retrunValue = new List<MachinePerformanceIndicator>();
            query.ForEach(
                t =>
                    {
                        retrunValue.AddRange(
                            t.Select(
                                n => new MachinePerformanceIndicator()
                                         {
                                             TotalDuration = n.Result.TotalDuration,
                                             TotalYiled = n.Result.TotalYiled,
                                             PerfectTime = n.Result.PerfectTime,
                                             ShiftDay = t.Key.ShiftDay,
                                             MachineId = t.Key.MachineId,
                                             ProductName = n.Result.ProductName,
                                             ProductId = n.Result.ProductId
                                         }));
                    });

            return retrunValue;
        }

        // 年-> 月的趋势， 月和周 -> 天的趋势
        public async Task<OeeDto> ListOeeDetailTendencyChart(OeeAnalysis input)
        {
            var paramter = await this.oeeAnalysisManager.FormartInputDto(input);
            switch (input.StatisticalWays)
            {
                case EnumStatisticalWays.ByWeek:
                    paramter.StatisticalWays = EnumStatisticalWays.ByDay;
                    break;
                case EnumStatisticalWays.ByMonth:
                    paramter.StatisticalWays = EnumStatisticalWays.ByWeek;
                    break;
                case EnumStatisticalWays.ByYear:
                    paramter.StatisticalWays = EnumStatisticalWays.ByMonth;
                    break;
            }

            var result = await this.ListMachineOEEChart(paramter);
            return result;
        }

        [HttpPost]
        public async Task<OeeDetailDailyItemDto> GetDetailDailyItem(OeeAnalysis input)
        {
            var paramter = await this.oeeAnalysisManager.FormartInputDto(input);

            if (input.StatisticalWays == EnumStatisticalWays.ByShift)
            {
                return await this.GetDetailShiftItem(paramter);
            }

            var availability = (await this.ListMachineAvailability(paramter)).FirstOrDefault();
            var qualityIndicator = (await this.ListQualityIndicatorsItemByProduct(paramter)).ToList();
            var performanceIndicator = (await this.ListPerformanceIndicatorsItemByProduct(paramter)).ToList();
            var unplannedPauses = await this.oeeRepository.ListUnplannedPause(paramter.MachineId, paramter.StartTime, paramter.EndTime.AddDays(1));
            var machineDetailYield = new MachineDetailYieldInfoInputDto()
                          {
                              MachineIdList = input.MachineIdList.ToList(),
                              SummaryDate = input.ShiftDay
                          };

            var gantt = await this.yieldAppService.GetMachineStatesGanttChart(machineDetailYield);
            var productionStatus = ObjectMapper.Map<MachineAvailabilityDto>(availability);
            if (productionStatus != null)
            {
                productionStatus.UnplannedPauses = ObjectMapper.Map<IEnumerable<UnplannedPauseDto>>(unplannedPauses);
            }

            // by product
            var qualityStatus = qualityIndicator
                .GroupBy(
                    q => new { Id = q.ProductId, Name = q.ProductName },
                    (key, g) => new { Product = key, DefatulItem = g.First(), List = g.ToList() }).Select(
                    q => new QualityStatusDto()
                             {
                                 MachineId = q.DefatulItem.MachineId,
                                 ShiftDay = q.DefatulItem.ShiftDay,
                                 ProductId = q.Product.Id,
                                 ProductName = q.Product.Name,
                                 TotalCount = q.List.Sum(n => n.TotalCount),
                                 UnqualifiedCount = q.List.Sum(n => n.UnqualifiedCount)
                             });

            var processingTimes = performanceIndicator
                .GroupBy(
                    q => new { Id = q.ProductId, Name = q.ProductName },
                    (key, g) => new { Product = key, List = g.ToList() }).Select(
                    q => new ProcessingTimeDto()
                             {
                                 ProductId = q.Product.Id,
                                 ProductName = q.Product.Name,
                                 ActualTime = q.List.Sum(n => n.ActualTime),
                                 PerfectTime = Math.Round(q.List.Sum(n => n.PerfectTime), 2) 
                             });

            return new OeeDetailDailyItemDto()
                       {
                           GanttChart = gantt.FirstOrDefault(),
                           ProductionStatus = productionStatus,
                           QualityStatus = qualityStatus,
                           ProcessingTimes = processingTimes
                       };
        }

        private async Task<OeeDto> ListShiftOee(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListShiftMachineOee(input);
            var query = (from av in result.Availability
                         join q in result.QualityIndicators on new { av.MachineId, av.ShiftDay } equals
                         new { q.MachineId, q.ShiftDay } into g
                         from k in g.DefaultIfEmpty()
                         join mp in result.Performance on new { av.MachineId, av.ShiftDay } equals
                         new { mp.MachineId, mp.ShiftDay } into pg
                         from kp in pg.DefaultIfEmpty()
                         select new { availability = av, qualityIndicators = k, performance = kp }).ToList();

            // 如果没有设定标准用时，performanceTime = 0 PRate = 0时为 按100% 计算
            var list = query.Select(
                n => new
                         {
                             n.availability,
                             AvRate =
                             n.availability.TotalDuration != 0
                                 ? Math.Round((n.availability.TotalDuration - n.availability.ReasonDuration) / n.availability.TotalDuration,4)
                                 : 1,
                             QRate = n.qualityIndicators != null && n.qualityIndicators.TotalCount != 0
                                         ? Math.Round(n.qualityIndicators.QualifiedCount / n.qualityIndicators.TotalCount, 4)
                                         : 1,
                             PRate = n.performance != null && n.performance.TotalYiled != 0 && n.performance.PerfectTime !=0
                                     && n.performance.TotalDuration != 0
                                         ? Math.Round(n.performance.PerfectTime / Math.Round(n.performance.TotalDuration / n.performance.TotalYiled, 2), 4) 
                                         : 1
                         }).ToList();

            var oeeList = list.Select(
                n => new MachineOEEDto()
                         {
                             MachineId = n.availability.MachineId,
                             MachineName = n.availability.MachineName,
                             ShiftDay = n.availability.ShiftDay,
                             Value = Math.Round(n.AvRate *n.QRate * n.PRate * 100, 2)
                         });

            return new OeeDto() { ShiftDayRanges = input.ShiftDayRanges, MachineOee = oeeList };
        }

        private async Task<OeeDetailDto> GetMachineShiftOeeDetail(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListShiftMachineOee(input);

            var totalDuration = result.Availability.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.TotalDuration);
            var totalReasonDuration = result.Availability.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.ReasonDuration);
            var availabilityRate = totalDuration != 0 ? (totalDuration - totalReasonDuration) / totalDuration : 1;

            var totalCount = result.QualityIndicators.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.TotalCount);
            var qualityIndicatorsRate = totalCount != 0
                                            ? result.QualityIndicators.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.QualifiedCount) / totalCount
                                            : 1;

            var performanceTotalDuration = result.Performance.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.TotalDuration);
            var performancePerfectTime = result.Performance.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.PerfectTime);
            var performanceTotalYiled = result.Performance.Where(r => r.ShiftDay == input.ShiftDay).Sum(r => r.TotalYiled);

            var performanceIndicatorsRate = performanceTotalDuration <= 0 || performanceTotalYiled <= 0 || performancePerfectTime <=0
                                                ? 1
                                                : performancePerfectTime
                                                  / Math.Round(performanceTotalDuration / performanceTotalYiled, 2);

            return new OeeDetailDto()
                       {
                           PerformanceIndicators = Math.Round(performanceIndicatorsRate, 4).ToString("P"),
                           QualityIndicators = Math.Round(qualityIndicatorsRate, 4).ToString("P"),
                           Availability = Math.Round(availabilityRate, 4).ToString("P")
                       };
        }

        private async Task<OeeDetailDailyItemDto> GetDetailShiftItem(OeeAnalysis input)
        {
            var result = await this.oeeRepository.ListShiftMachineOeeByProduct(input);
            var availability = result.Availability.FirstOrDefault(r => r.ShiftDay == input.ShiftDay);
            var qualityIndicator = result.QualityIndicators.Where(r => r.ShiftDay == input.ShiftDay).ToList();
            var performanceIndicator = result.Performance.Where(r => r.ShiftDay == input.ShiftDay).ToList();

            var dateTimeRange = (await this.oeeRepository.ListShiftDateTimeRange(new OeeAnalysis() { StartTime = input.StartTime, EndTime = input.EndTime }))
                .FirstOrDefault(n => n.ShiftDay == input.ShiftDay);

            var machineDetailYield = new MachineDetailYieldInfoInputDto()
                    {
                        MachineIdList = input.MachineIdList.ToList(),
                        StartTime = dateTimeRange?.StartTime,
                        EndTime = dateTimeRange?.EndTime,
                        SummaryDate = dateTimeRange?.Date.ToString("yyyy-MM-dd")
                    };

            var gantt = await this.yieldAppService.GetMachineStatesGanttChart(machineDetailYield);
            var productionStatus = new MachineAvailabilityDto();
            if (availability != null)
            {
                var unplannedPauses = await this.oeeRepository.ListUnplannedPause(input.MachineId, machineDetailYield.StartTime, machineDetailYield.EndTime);
                productionStatus.UnplannedPauses = ObjectMapper.Map<IEnumerable<UnplannedPauseDto>>(unplannedPauses);
                productionStatus.MachineId = availability.MachineId;
                productionStatus.ActualWorkTime = availability.TotalDuration - availability.ReasonDuration;
                productionStatus.PlannedWorkTime = availability.TotalDuration;
                productionStatus.ShiftDay = availability.ShiftDay;
            }               

            // by product
            var qualityStatusResult = new List<QualityStatusDto>();
            var qualityStatus = qualityIndicator
                .GroupBy(
                    q => new { Id = q.ProductId, Name = q.ProductName },
                    (key, g) => new { Product = key, DefatulItem = g.First(), List = g.ToList() })
                    .ToList();

            qualityStatus.ForEach(
                q => qualityStatusResult.Add(new QualityStatusDto()
                        {
                            MachineId = q.DefatulItem.MachineId,
                            ShiftDay = q.DefatulItem.ShiftDay,
                            ProductId = q.Product.Id,
                            ProductName = q.Product.Name,
                            TotalCount = q.List.Sum(n => n.TotalCount),
                            UnqualifiedCount = q.List.Sum(n => n.UnqualifiedCount)
                        }));

            var processingTimesResult = new List<ProcessingTimeDto>();
            var processingTimes = performanceIndicator.GroupBy(
                q => new { Id = q.ProductId, Name = q.ProductName },
                (key, g) => new { Product = key, List = g.ToList() }).ToList();


            processingTimes.ForEach(q => processingTimesResult.Add(
                new ProcessingTimeDto()
                    {
                        ProductId = q.Product.Id,
                        ProductName = q.Product.Name,
                        ActualTime = q.List.Sum(n => n.TotalYiled) == 0? 1: Math.Round(q.List.Sum(n => n.TotalDuration) / q.List.Sum(n => n.TotalYiled),2),
                        PerfectTime = Math.Round(q.List.Sum(n => n.PerfectTime), 2)
                    }));

            return new OeeDetailDailyItemDto()
                       {
                           GanttChart = gantt.FirstOrDefault(),
                           ProductionStatus = productionStatus,
                           QualityStatus = qualityStatusResult,
                           ProcessingTimes = processingTimesResult
                       };
        }
    }
}