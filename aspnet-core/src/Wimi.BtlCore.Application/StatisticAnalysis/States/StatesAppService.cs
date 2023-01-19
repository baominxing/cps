using Abp.Auditing;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.StatisticAnalysis.States.Dto;
using Wimi.BtlCore.StatisticAnalysis.States.Export;

namespace Wimi.BtlCore.StatisticAnalysis.States
{
    public class StatesAppService : BtlCoreAppServiceBase, IStatesAppService
    {
        private readonly IStatesExporter exporter;
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<StateInfo> statusInfoRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly MongoMachineManager mongoMacineManager;
        private readonly IMachineManager machineManager;
        private readonly ShiftCalendarManager shiftCalendarManager;
        private readonly IRepository<ShiftSolution> shiftSolutionsRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly IStateRepository stateRepository;

        public StatesAppService(
              IStatesExporter exporter,
              IRepository<StateInfo> statusInfoRepository,
              ICommonLookupAppService commonLookupAppService,
              IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
              IRepository<Machine> machineRepository,
              MongoMachineManager mongoMacineManager,
              ShiftCalendarManager shiftCalendarManager,
              IMachineManager machineManager,
              IRepository<ShiftSolution> shiftSolutionsRepository,
              IRepository<ArchiveEntry> archiveEntryRepository,
               IStateRepository stateRepository
              )
        {
            this.exporter = exporter;
            this.statusInfoRepository = statusInfoRepository;
            this.commonLookupAppService = commonLookupAppService;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineRepository = machineRepository;
            this.mongoMacineManager = mongoMacineManager;
            this.shiftCalendarManager = shiftCalendarManager;
            this.shiftSolutionsRepository = shiftSolutionsRepository;
            this.machineManager = machineManager;
            this.archiveEntryRepository = archiveEntryRepository;
            this.stateRepository = stateRepository;
        }

        /// <summary>
        /// 获取所选时间段内的设备使用比例(按设备)
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<IEnumerable<MachineStateRateDto>> GetMachineStateRateByMac(GetMachineStateRateInputDto input)
        {
            var returnValue = new List<MachineStateRateDto>();
            var machineIdList = input.MachineIdList;


            if (input.MachineIdList == null || !input.MachineIdList.Any())
            {
                if (input.QueryType == "1")
                {
                    var query = await machineDeviceGroupRepository.GetAll().Select(m => new { m.DeviceGroupId, m.MachineId }).ToListAsync();
                    input.MachineIdList = query.Select(s => s.DeviceGroupId).Distinct().ToList();
                    machineIdList = query.Select(s => s.MachineId).ToList();
                }
                else
                {
                    var query = (await this.machineManager.ListDefaultSearchMachines()).Select(t => t.Value).ToList();
                    input.MachineIdList = machineIdList = query;
                }
            }

            if (input.QueryType == "1")
            {
                var query = await machineDeviceGroupRepository.GetAll().Where(m => input.MachineIdList.Contains(m.DeviceGroupId)).Select(m => m.MachineId).ToListAsync();
                machineIdList = query;
            }

            var startTime = Convert.ToDateTime(input.StartTime);
            var endTime = Convert.ToDateTime(input.EndTime);

            //根据传入的班次方案名获取shiftSolutionId
            var shiftSolutionIdList = await GetshiftSolutionIdListByName(input.MachineShiftSolutionNameList);

            // 获取正确的所选日期
            var correctedQueryDateList = (await shiftCalendarManager.CorrectQueryDate(startTime, endTime, input.StatisticalWay, machineIdList, shiftSolutionIdList)).ToList();

            input.StartTime = correctedQueryDateList.Count() == 0 ? input.StartTime : correctedQueryDateList.First().ShiftDay;
            input.EndTime = correctedQueryDateList.Count() == 0 ? input.EndTime : correctedQueryDateList.Last().ShiftDay;

            returnValue = await stateRepository.GetMachineStateRateData(input);

            var result = CorrectListForChart(returnValue);

            return result;
        }

        private IEnumerable<MachineStateRateDto> CorrectListForChart(List<MachineStateRateDto> inputList)
        {
            var querySummaryDates = new List<string>(); // 返回结果集中的所包含的日期范围
            var keys = new List<string>(); // 返回结果集中的所包含设备加删程序号
            for (var i = 0; i < inputList.Count; i++)
            {
                if (!querySummaryDates.Contains(inputList[i].SummaryDate))
                {
                    querySummaryDates.Add(inputList[i].SummaryDate);
                }

                keys.Add(inputList[i].SummaryId + "|" + inputList[i].SummaryName);
            }

            // 根据日期对当日没有利用率的设备补0,用于前台echarts加载数据
            for (var i = 0; i < querySummaryDates.Count; i++)
            {
                for (var j = 0; j < keys.Count; j++)
                {
                    if (
                        inputList.Count(
                            s =>
                            s.SummaryDate + "|" + s.SummaryId + "|" + s.SummaryName
                            == querySummaryDates[i] + "|" + keys[j]) == 0)
                    {
                        inputList.Add(
                            new MachineStateRateDto
                            {
                                SummaryDate = querySummaryDates[i],
                                SummaryId = Convert.ToInt32(keys[j].Split('|')[0]),
                                SummaryName = keys[j].Split('|')[1],
                                StopDurationRate = 0,
                                RunDurationRate = 0,
                                FreeDurationRate = 0,
                                OfflineDurationRate = 0,
                                DebugDurationRate = 0
                            });
                    }
                }
            }

            inputList.ForEach(
                s =>
                {
                    var rateCount = s.StopDurationRate + s.RunDurationRate + s.FreeDurationRate + s.DebugDurationRate;
                    s.OfflineDurationRate = rateCount > 1 ? 0 : 1 - rateCount;
                });

            var outputList = inputList;

            return outputList;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> Export(GetMachineStateRateInputDto input)
        {
            var data = await this.GetMachineStateRateByMac(input);
            return exporter.ExportToFile(data);
        }

        /// <summary>
        /// 获取设备实时状态记录
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        [HttpPost]
        public async Task<RealtimeMachineStateSummaryDto> GetRealtimeMachineStateSummary()
        {
            try
            {
                var deviceGroupWithPermissions =
                    await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

                var machines =
                    deviceGroupWithPermissions.Machines.Where(
                        g => deviceGroupWithPermissions.GrantedGroupIds.Contains(g.DeviceGroupId))
                        .Select(m => m.Code)
                        .ToList();

                var mongoData = mongoMacineManager.ListOriginalMongoMachines(machines).ToList();

                var results = (from m in mongoData
                               group m by m.State.Code into g
                               select new
                               {
                                   state = g.Key,
                                   count = g.Count()
                               }).ToList();

                var statusInfo = this.statusInfoRepository
                    .GetAll().Where(q => q.Type == EnumMachineStateType.State)
                    .ToList();

                var summary = new RealtimeMachineStateSummaryDto();

                var totalMachineCount = 0;

                foreach (var item in statusInfo)
                {
                    var target = results.FirstOrDefault(q => q.state == item.Code);
                    if (target == null || target.count == 0)
                    {
                        summary.StateCollection.Add(new StateItem { Code = item.Code, Name = item.DisplayName, Amount = 0, Hexcode = item.Hexcode });
                        continue;
                    }

                    int amount;
                    if (!int.TryParse(target.count.ToString(), out amount))
                    {
                        continue;
                    }

                    totalMachineCount += amount;

                    summary.StateCollection.Add(new StateItem { Code = item.Code, Name = item.DisplayName, Amount = amount, Hexcode = item.Hexcode });
                }

                summary.StateCollection.ForEach(q => q.Rate = totalMachineCount > 0 ? Math.Round(q.Amount / (decimal)totalMachineCount * 100, 2) : 0);

                return summary;

            }
            catch (Exception ex)
            {
                const string ErrorMessage = "Mongodb数据库连接失败";
                this.Logger.Fatal(ErrorMessage, ex);
                return new RealtimeMachineStateSummaryDto()
                {
                    IsError = true,
                    Message = ErrorMessage
                };
            }
        }
 
        /// <summary>
        ///     获取设备班次方案信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input)
        {
            var param = new { StartTime = Convert.ToDateTime(input.startTime), EndTime = Convert.ToDateTime(input.endTime), MachineIdList = (List<int>)input.machineIdList.ToObject<List<int>>(), QueryType = Convert.ToInt32(input.queryType) };
            var machineIds = param.MachineIdList;
            if (param.QueryType == 1)
            {
                machineIds = machineDeviceGroupRepository.GetAll().Where(x => param.MachineIdList.Contains(x.DeviceGroupId)).Select(x => x.MachineId).ToList();
            }
            return input.machineIdList.Count == 0 ? new List<GetMachineShiftSolutionsDto>() : (await this.shiftCalendarManager.GetMachineShiftSolutions(param.StartTime, param.EndTime, machineIds));
        }

        private async Task<List<int>> GetshiftSolutionIdListByName(List<string> machineShiftSolutionNameList)
        {
            var query = await (from s in shiftSolutionsRepository.GetAll()
                               where machineShiftSolutionNameList.Contains(s.Name)
                               select s.Id
                                ).ToListAsync();

            return query;
        }
 
    }
}