namespace Wimi.BtlCore.StatisticAnalysis.Alarms
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Abp.Configuration;
    using Abp.Domain.Repositories;
    using Abp.Extensions;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using Common;
    using Dapper;
    using Dto;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Wimi.BtlCore.Archives;
    using Wimi.BtlCore.BasicData.Alarms;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
    using Wimi.BtlCore.BasicData.Shifts.Manager;
    using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.StatisticAnalysis.Alarms.Export;

    public class AlarmsAppService : BtlCoreAppServiceBase, IAlarmsAppService
    {
        private readonly IRepository<AlarmInfo, long> alarmInfoRepository;
        private readonly IAlarmRepository alarmRepository;
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<Machine> machineRepository;
        private readonly ShiftCalendarManager shiftCalendarManager;
        private readonly IAlarmExporter alarmExporter;
        private readonly IMachineManager machineManager;
        private readonly ISettingManager settingManager;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;

        public AlarmsAppService(
            IAlarmRepository alarmRepository,
            IRepository<AlarmInfo, long> alarmInfoRepository,
            ICommonLookupAppService commonLookupAppService,
            IRepository<Machine> machineRepository,
            ShiftCalendarManager shiftCalendarManager,
            IAlarmExporter alarmExporter,
            IMachineManager machineManager,
            ISettingManager settingManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.alarmRepository = alarmRepository;
            this.alarmInfoRepository = alarmInfoRepository;
            this.commonLookupAppService = commonLookupAppService;
            this.machineRepository = machineRepository;
            this.shiftCalendarManager = shiftCalendarManager;
            this.alarmExporter = alarmExporter;
            this.machineManager = machineManager;
            this.settingManager = settingManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.archiveEntryRepository = archiveEntryRepository;
        }

        [HttpPost]
        public async Task DeleteAlarmInfo(NullableIdDto input)
        {
            if (input.Id.HasValue) await this.alarmInfoRepository.DeleteAsync(input.Id.Value);
        }

        /// <summary>
        ///     获取报警数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmChartData(GetMachineAlarmsInputDto input)
        {
            var result = new List<MachineAlarmStatisticesDto>();

            if (input.MachineIdList.Count == 0) return result;

            EnumStatisticalWays enmuStatisticalWay;

            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
            {
                enmuStatisticalWay = EnumStatisticalWays.ByDay;
            }

            var correctedQueryDateList = await this.shiftCalendarManager.CorrectQueryDate((DateTime)input.StartTime, (DateTime)input.EndTime, enmuStatisticalWay, input.MachineIdList, input.MachineShiftSolutionIdList);

            if (correctedQueryDateList.Count() != 0)
            {
                input.StartTime = Convert.ToDateTime(correctedQueryDateList.FirstOrDefault()?.ShiftDay ?? ((DateTime)input.StartTime).ToString("yyyy-MM-dd"));
                input.EndTime = Convert.ToDateTime(correctedQueryDateList.LastOrDefault()?.ShiftDay ?? ((DateTime)input.EndTime).ToString("yyyy-MM-dd"));
            }
            //根据查询时间范围，获取需要union的分表
            input.UnionTables = this.GetUnionTables(input);

            var machineAlarms = await this.alarmRepository.GetAlarmChartData(ObjectMapper.Map<GetMachineAlarms>(input));

            ObjectMapper.Map<IEnumerable<MachineAlarmStatistices>, IEnumerable<MachineAlarmStatisticesDto>>(machineAlarms, result);

            return await this.AdjustListForEChart(result, input);
        }

        private List<string> GetUnionTables(GetMachineAlarmsInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "Alarms").ToList()
                .Where(s => input.StartTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= input.EndTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }

        /// <summary>
        ///     获取报警数据记录数量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> GetAlarmChartDataCount(GetMachineAlarmsInputDto input)
        {
            if (input.MachineIdList.Count == 0) return 0;

            EnumStatisticalWays enmuStatisticalWay;

            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
            {
                enmuStatisticalWay = EnumStatisticalWays.ByDay;
            }

            var correctedQueryDateList = await this.shiftCalendarManager.CorrectQueryDate((DateTime)input.StartTime, (DateTime)input.EndTime, enmuStatisticalWay, input.MachineIdList, input.MachineShiftSolutionIdList);

            if (correctedQueryDateList.Count() > 0)
            {
                input.StartTime = Convert.ToDateTime(correctedQueryDateList.FirstOrDefault().ShiftDay);
                input.EndTime = Convert.ToDateTime(correctedQueryDateList.LastOrDefault().ShiftDay);
            }
            //根据查询时间范围，获取需要union的分表
            input.UnionTables = this.GetUnionTables(input);

            return await this.alarmRepository.GetAlarmChartDataCount(ObjectMapper.Map<GetMachineAlarms>(input));
        }

        /// <summary>
        ///     获取点击设备的报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmDetailData(GetMachineAlarmsInputDto input)
        {
            if (input.MachineIdList.Count == 0) return new List<MachineAlarmStatisticesDto>();

            //根据查询时间范围，获取需要union的分表
            input.UnionTables = this.GetUnionTables(input);

            var query = (await this.alarmRepository.GetAlarmDetailData(ObjectMapper.Map<GetMachineAlarms>(input)));
            return ObjectMapper.Map<IEnumerable<MachineAlarmStatisticesDto>>(query);
        }

        /// <summary>
        ///     获取点击详细信息按钮弹出框数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmDetailDataForModal(GetMachineAlarmsInputDto input)
        {
            if (input.MachineIdList.Count == 0) return new List<MachineAlarmStatisticesDto>();
            input.UnionTables = this.GetUnionTables(input);
            return ObjectMapper.Map<List<MachineAlarmStatisticesDto>>((await this.alarmRepository.GetAlarmDetailDataForModal(ObjectMapper.Map<GetMachineAlarms>(input))));
        }

        [HttpPost]
        public async Task<PagedResultDto<CreateOrEditAlarmInfoDto>> GetAlarmInfoList(GetAlarmInfoListInputDto input)
        {
            var query = from a in this.alarmInfoRepository.GetAll()
                        join m in this.machineRepository.GetAll() on a.MachineId equals m.Id
                        select new CreateOrEditAlarmInfoDto
                        {
                            Id = a.Id,
                            MachineId = m.Id,
                            MachineCode = m.Code,
                            MachineName = m.Name,
                            Code = a.Code,
                            Message = a.Message,
                            CreationTime = a.CreationTime,
                            CreatorUserId = a.CreatorUserId,
                            SortSeq = m.SortSeq
                        };

            query = query.WhereIf(
                !input.Search.Value.IsNullOrWhiteSpace(),
                s => s.Code.Contains(input.Search.Value)
                || s.Message.Contains(input.Search.Value)
                || s.MachineCode.Contains(input.Search.Value)
                || s.MachineName.Contains(input.Search.Value));

            var returnValue = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var count = await query.CountAsync();

            return new DatatablesPagedResultOutput<CreateOrEditAlarmInfoDto>(count, returnValue, count)
            {
                Draw = input.Draw
            };
        }

        /// <summary>
        ///     页面加载获取默认数据:当日登录账号下有权限的设备的报警次数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineAlarmStatisticesDto>> GetDefaultAlarmChartData(
            GetMachineAlarmsInputDto input)
        {
            // 获取当前登录账号权限能看到的设备列表
            var machineListWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            if (input.MachineIdList.Count == 0)
            {
                var defaultMachineCount = AppSettings.DefaultSearchMachineCount;
                input.MachineIdList = machineListWithPermissions.Machines.OrderBy(p => p.DeviceGroupId).ThenBy(p => p.SortSeq).ThenBy(p => p.Code).Take(defaultMachineCount).Select(s => s.Id).ToList();
            }
            else
            {
                input.MachineIdList = machineListWithPermissions.Machines.Select(s => s.Id).ToList();
            }

            return await this.GetAlarmChartData(input);
        }

        /// <summary>
        ///     获取设备班次方案信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input)
        {
            var param = new { StartTime = Convert.ToDateTime(input.startTime), EndTime = Convert.ToDateTime(input.endTime), MachineIdList = (List<int>)input.machineIdList.ToObject<List<int>>() };
 
            return input.machineIdList.Count == 0 ? new List<GetMachineShiftSolutionsDto>() : (await this.shiftCalendarManager.GetMachineShiftSolutions(param.StartTime, param.EndTime, param.MachineIdList));
        }

        public async Task ImportDataByCover(ImportDataDto input)
        {
            if (input.ImportData.Any(s => string.IsNullOrEmpty(s.Code) || string.IsNullOrEmpty(s.Message)))
            {
                throw new UserFriendlyException("导入数据存在报警编号为空或报警内容为空的记录");
            }

            await alarmRepository.ImportDataByCover(input);
        }

        public async Task ImportDataByIncrement(ImportDataDto input)
        {
            if (input.ImportData.Any(s => string.IsNullOrEmpty(s.Code) || string.IsNullOrEmpty(s.Message)))
            {
                throw new UserFriendlyException("导入数据存在报警编号为空或报警内容为空的记录");
            }

            await alarmRepository.ImportDataByIncrement(input);
        }

        [HttpPost]
        public async Task UpdateOrEditAlarmInfo(CreateOrEditAlarmInfoDto input)
        {
            try
            {
                var count = await this.alarmInfoRepository.GetAll().WhereIf(input.Id.HasValue, q => q.Id != input.Id.Value)
                    .CountAsync(a => a.Code.ToLower().Equals(input.Code.ToLower()) && a.MachineId == input.MachineId);

                if (count > 0)
                    throw new UserFriendlyException(this.L("MachineAlreadyExistRecord{0}", input.Code));

                var alarmInfo = ObjectMapper.Map<AlarmInfo>(input);
                await this.alarmInfoRepository.InsertOrUpdateAsync(alarmInfo);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(this.L("SaveFailed{0}", ex.Message));
            }
        }

        public async Task<FileDto> Export(GetMachineAlarmsInputDto input)
        {
            var query = await alarmRepository.QueryAlarmExportData(input.StatisticalWay,input.StartTime,input.EndTime,input.MachineIdList,input.MachineShiftSolutionIdList);
            return this.alarmExporter.ExportToFile(query);
        }

        /// <summary>
        ///为了前台EChart显示正确,需要针对当天没有数据的设备补零
        /// </summary>
        /// <param name="result">
        /// </param>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        /// </returns>
        private async Task<IEnumerable<MachineAlarmStatisticesDto>> AdjustListForEChart(List<MachineAlarmStatisticesDto> result,GetMachineAlarmsInputDto input)
        {
            var querySummaryDates = new List<MachineAlarmStatisticesDto>();

            // 获取查询结果集中的所包含的日期范围
            foreach (var machineAlarmStatisticesDto in result)
                if (!querySummaryDates.Any(p => p.SummaryDate == machineAlarmStatisticesDto.SummaryDate))
                    querySummaryDates.Add(new MachineAlarmStatisticesDto()
                    {
                        SummaryDate = machineAlarmStatisticesDto.SummaryDate,
                        ShiftItemSeq = machineAlarmStatisticesDto.ShiftItemSeq,
                        ShiftDay = machineAlarmStatisticesDto.ShiftDay
                    });

            // 获取所选设备的设备信息
            var queriedMachineInfos = ObjectMapper.Map<List<MachineAlarmStatisticesDto>>((await this.alarmRepository.GetQueriedMachineInfo(ObjectMapper.Map<GetMachineAlarms>(input))));

            var keys = new List<string>();

            foreach (var machineAlarmStatisticesDto in queriedMachineInfos)
            {
                var key = machineAlarmStatisticesDto.MachineId + "|" + machineAlarmStatisticesDto.MachineName + "|"
                          + machineAlarmStatisticesDto.MachineGroupName;
                if (!keys.Contains(key)) keys.Add(key);
            }

            // 根据日期对当日没有产出的设备程序号补0,用于前台echarts加载数据
            for (var i = 0; i < querySummaryDates.Count; i++)
            {
                for (var j = 0; j < keys.Count; j++)
                {
                    if (result.All(s => s.SummaryDate + "|" + s.MachineId + "|" + s.MachineName + "|" + s.MachineGroupName != querySummaryDates[i].SummaryDate + "|" + keys[j]))
                        result.Add(new MachineAlarmStatisticesDto
                        {
                            SummaryDate = querySummaryDates[i].SummaryDate,
                            MachineId = keys[j].Split('|')[0],
                            MachineName = keys[j].Split('|')[1],
                            MachineGroupName = keys[j].Split('|')[2],
                            AlarmCount = "0",
                            ShiftDay = querySummaryDates[i].ShiftDay,
                            ShiftItemSeq = querySummaryDates[i].ShiftItemSeq,
                        });
                }
            }

            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            if (enmuStatisticalWay == EnumStatisticalWays.ByShift)
            {
                //班次查 按 时间 班次Seq 降序排
                return result.OrderByDescending(p => p.ShiftDay.Date.ToString("yyyy-MM-dd") + (/*int.MaxValue - */p.ShiftItemSeq));

            }
            else
            {
                return result.OrderByDescending(s => s.SummaryDate);
            }
        }
    }
}