using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.EfficiencyTrendas;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Export;
using Wimi.BtlCore.SummaryStatistics;

namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends
{
    [AbpAuthorize(PermissionNames.Pages_EfficiencyTrends)]
    public class EfficiencyTrendsAppService : BtlCoreAppServiceBase, IEfficiencyTrendsAppService
    {
        private readonly IEfficiencyTrendsExporter exporter;
        private readonly IActivationRepository activationRepository;
        private readonly IShiftCalendarManager shiftCalendarManager;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<ShiftSolution> shiftSolutionRepository;
        private readonly ISettingManager settingManager;
        private readonly IEfficiencyTrendaRepository efficiencyTrendaRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;

        public EfficiencyTrendsAppService(
            IActivationRepository activationRepository,
            IShiftCalendarManager shiftCalendarManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            ICommonLookupAppService commonLookupAppService,
            IRepository<ShiftSolution> shiftSolutionRepository,
            IEfficiencyTrendsExporter exporter,
            ISettingManager settingManager,
            IRepository<ArchiveEntry> archiveEntryRepository,
            IEfficiencyTrendaRepository efficiencyTrendaRepository)
        {
            this.exporter = exporter;
            this.activationRepository = activationRepository;
            this.shiftCalendarManager = shiftCalendarManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.commonLookupAppService = commonLookupAppService;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.settingManager = settingManager;
            this.archiveEntryRepository = archiveEntryRepository;
            this.efficiencyTrendaRepository = efficiencyTrendaRepository;
        }

        /// <summary>
        ///     获取设备班次方案信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input)
        {
            var queryType = Convert.ToInt32(input.queryType);

            var queryMachineIds = (List<int>)input.machineId.ToObject<List<int>>();

            var param = new { StartTime = Convert.ToDateTime(input.startTime), EndTime = Convert.ToDateTime(input.endTime), MachineIdList = queryMachineIds };

            if (queryType == 0)
            {
                return input.machineId.Count == 0 ? new List<GetMachineShiftSolutionsDto>() : (await this.shiftCalendarManager.GetMachineShiftSolutions(param.StartTime, param.EndTime, param.MachineIdList));
            }
            else
            {
                return input.machineId.Count == 0 ? new List<GetMachineShiftSolutionsDto>() : (await this.shiftCalendarManager.GetMachineGroupShiftSolutions(param.StartTime, param.EndTime, param.MachineIdList));
            }
        }

        [HttpPost]
        public List<EfficiencyTrendasDataTablesDto> GetEfficiencyTrendasDataTablesColumns(EfficiencyTrendsInputDto input)
        {
            var columnList = new List<EfficiencyTrendasDataTablesDto>();

            EnumStatisticalWays enmuStatisticalWays;
            if (!Enum.TryParse(input.StatisticalWays, out enmuStatisticalWays))
                enmuStatisticalWays = EnumStatisticalWays.ByDay;

            columnList = efficiencyTrendaRepository.GetEfficiencyTrendasDataTablesColumns(input);

            if (columnList.Count > 0)
            {
                var groupBytitle = string.Empty;
                switch (enmuStatisticalWays)
                {
                    case EnumStatisticalWays.ByShift:
                        groupBytitle = L("StatisticalShift");
                        break;
                    case EnumStatisticalWays.ByDay:
                        groupBytitle = L("StatisticalDate");
                        break;
                    case EnumStatisticalWays.ByWeek:
                        groupBytitle = L("StatisticalWeek");
                        break;
                    case EnumStatisticalWays.ByMonth:
                        groupBytitle = L("StatisticalMonth");
                        break;
                    case EnumStatisticalWays.ByYear:
                        groupBytitle = L("StatisticalYear");
                        break;
                }

                columnList.Insert(0, new EfficiencyTrendasDataTablesDto
                {
                    Data = "dimensions",
                    Title = groupBytitle,
                    ClassName = string.Empty
                });
            }

            return columnList;
        }

        [HttpPost]
        public async Task<ListResultDto<EfficiencyTrendasDataTablesDataDto>> GetEfficiencyTrendasList(EfficiencyTrendsInputDto input)
        {
            if (input.MachineId.Count == 0) return new ListResultDto<EfficiencyTrendasDataTablesDataDto>();

            //根据查询时间范围，获取需要union的分表
            var unionTables = this.GetUnionTables(input);

            EnumStatisticalWays enmuStatisticalWays;
            if (!Enum.TryParse(input.StatisticalWays, out enmuStatisticalWays))
                enmuStatisticalWays = EnumStatisticalWays.ByDay;

            EnumEfficiencyTrendsGroupType groupBy;
            if (!Enum.TryParse(input.GroupType, out groupBy)) groupBy = EnumEfficiencyTrendsGroupType.UsingRate;

            var tablesData = new List<EfficiencyTrendasDataTablesDataDto>();

            var result = await efficiencyTrendaRepository.GetEfficiencyTrendasExpandoObject(enmuStatisticalWays, input.MachineId, input.MachineShiftDetailId,
                input.MachineShiftSolutionNameList, input.QueryType, input.StartTime, input.EndTime, unionTables);

            if (enmuStatisticalWays == EnumStatisticalWays.ByShift)
            {
                foreach (var item in result)
                {
                    var shiftName = item.FirstOrDefault(i => i.Key == "ShiftItemName").Value;
                    var shiftDay = item.FirstOrDefault(i => i.Key == "dimensions").Value;
                    var returnValue = new EfficiencyTrendasDataTablesDataDto();
                    foreach (var ite in item)
                    {
                        if (ite.Key == "dimensions")
                        {
                            returnValue.RateData.Add(ite.Key, shiftDay + "-" + shiftName);

                        }
                        else if (!(ite.Key == "ShiftItemName" || ite.Key == "ShiftItemId"))
                        {
                            returnValue.RateData.Add(ite.Key, ite.Value == null ? "0.00" : ite.Value.ToString());
                        }

                    }
                    tablesData.Add(returnValue);
                }
            }
            else
            {
                foreach (var item in result)
                {
                    var returnValue = new EfficiencyTrendasDataTablesDataDto();
                    item.ToList().ForEach(t => returnValue.RateData.Add(t.Key, t.Value == null ? "0.00" : t.Value.ToString()));
                    tablesData.Add(returnValue);
                }
            }

            return new ListResultDto<EfficiencyTrendasDataTablesDataDto>(tablesData);
        }

        [HttpPost]
        public async Task<ListResultDto<EfficiencyTrendasDataTablesDataDto>> GetMachineActivation(EfficiencyTrendsInputDto input)
        {
            var tablesData = new List<EfficiencyTrendasDataTablesDataDto>();
            input.UnionTables = this.GetUnionTables(input);
            foreach (var item in await this.activationRepository.GetMachineActivationOriginalData(input))
            {
                var returnValue = new EfficiencyTrendasDataTablesDataDto();
                item.ForEach(t => returnValue.RateData.Add(t.Key.ToLower(), t.Value?.ToString() ?? "0.00"));
                tablesData.Add(returnValue);
            }

            return new ListResultDto<EfficiencyTrendasDataTablesDataDto>(tablesData);
        }

        private List<string> GetUnionTables(EfficiencyTrendsInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => input.StartTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= input.EndTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }

        [HttpPost]
        public async Task<IEnumerable<NameValueDto<int>>> GetDefaultMachines()
        {
            // 获取当前登录账号权限能看到的设备列表
            var machineListWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndDefaultCountMachineWithPermissions();
            var result = machineListWithPermissions.Machines.Select(s => new NameValueDto<int>()
            {
                Name = s.Name,
                Value = s.Id
            }).ToList();
            return result;
        }

        public async Task<FileDto> Export(EfficiencyTrendsInputDto input)
        {
            var data = await this.GetHistoryParamtersForExport(input);
            return exporter.ExportToFile(data);
        }

        private async Task<GetEfficiencyTrendsListDto> GetHistoryParamtersForExport(EfficiencyTrendsInputDto input)
        {
            var result = new GetEfficiencyTrendsListDto();

            var columnList = this.GetEfficiencyTrendasDataTablesColumns(input);
            result.EfficiencyTrendsColumns = columnList;

            if (input.GroupType == "2")
            {
                result.EfficiencyTrendsData = (await this.GetMachineActivation(input));
                result.GroupTypeName = this.L("CropMobility");
            }
            else
            {
                result.EfficiencyTrendsData = (await this.GetEfficiencyTrendasList(input));
                result.GroupTypeName = this.L("OperatingRate");
            }

            return result;
        }

        private static dynamic ToExpandoDynamic(object value)
        {
            var dapperRowProperties = value as IDictionary<string, object>;
            IDictionary<string, object> expando = new ExpandoObject();

            if (dapperRowProperties != null)
                foreach (var property in dapperRowProperties) expando.Add(property.Key, property.Value);

            return expando as ExpandoObject;
        }

    }
}