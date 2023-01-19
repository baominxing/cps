using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.QualifiedStatistics;
using Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto;
using Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Export;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics
{
    public class QualifiedStatisticsAppService : BtlCoreAppServiceBase, IQualifiedStatisticsAppService
    {
        private readonly IQualifiedStatisticsExporter exporter;
        private readonly IQualifiedStatisticsRepository qualifiedStatisticsRepository;
        private readonly IQualifiedStatisticsManager qualifiedStatisticsManager;
        private readonly IShiftCalendarManager shiftCalendarManager;
        private readonly IMachineManager machineManager;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;

        public QualifiedStatisticsAppService(IQualifiedStatisticsExporter exporter,
            IQualifiedStatisticsRepository qualifiedStatisticsRepository,
            QualifiedStatisticsManager qualifiedStatisticsManager,
            IShiftCalendarManager shiftCalendarManager,
            IMachineManager machineManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.exporter = exporter;
            this.qualifiedStatisticsRepository = qualifiedStatisticsRepository;
            this.qualifiedStatisticsManager = qualifiedStatisticsManager;
            this.shiftCalendarManager = shiftCalendarManager;
            this.machineManager = machineManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.archiveEntryRepository = archiveEntryRepository;
        }

        public async Task<ListQualificationInfoResultDto> ListQualificationInfo(GetDeviceGroupQualifiedRateRequestDto input)
        {
            var startDate = Convert.ToDateTime(input.StartTime);
            var endDate = Convert.ToDateTime(input.EndTime);
            var machineListId = (await this.machineManager.ListMachines()).Select(t => t.Value).ToList();
            var dateList = (await this.shiftCalendarManager.CorrectQueryDate(startDate, endDate, input.StatisticalWay,
                machineListId, input.ShiftSolutionIdList)).ToList();
            if (!dateList.Any())
            {
                return new ListQualificationInfoResultDto();
            }

            var startTime = dateList.First().ShiftDay;
            var endTime = dateList.Last().ShiftDay;
            input.UnionTables = this.GetUnionTables(input);
            var query = await qualifiedStatisticsRepository.ListQualification(input.StatisticalWay, startTime, endTime,
                input.DeviceGroupIdList, input.ShiftSolutionIdList, input.UnionTables);

            var summaryDateLists = query.Select(q => q.SummaryDate).Distinct().ToList();

            var tableData = query.GroupBy(q => q.DisplayName, (key, g) => new { key, List = g.ToList() }).Select(t =>
                  new QualifiedStatisticsDto
                  {
                      DisplayName = t.key,
                      Items = t.List
                  });
            return new ListQualificationInfoResultDto()
            {
                SumarryDateList = summaryDateLists,
                TableData = tableData
            };
        }

        public async Task<FileDto> Export(GetDeviceGroupQualifiedRateRequestDto input)
        {
            var data = await this.ListQualificationInfo(input);
            return exporter.ExportToFile(data.TableData);
        }

        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> ListDeviceGroupSolution(GetDeviceGroupQualifiedRateRequestDto input)
        {
            var startDate = Convert.ToDateTime(input.StartTime);
            var endDate = Convert.ToDateTime(input.EndTime);

            var machineIdList = new List<int>();
            if (input.DeviceGroupIdList.Any())
            {
                machineIdList = await this.machineDeviceGroupRepository.GetAll()
                    .Where(d => input.DeviceGroupIdList.Contains(d.DeviceGroupId)).Select(t => t.MachineId)
                    .ToListAsync();
            }
            else
            {
                machineIdList = (await this.machineManager.ListMachines()).Select(t => t.Value).ToList();
            }

            var query = await this.shiftCalendarManager.GetMachineShiftSolutions(startDate, endDate, machineIdList);

            return query.GroupBy(t => new { t.MachineShiftSolutionId, t.MachineShiftSolutionName }).Select(t =>
                  new GetMachineShiftSolutionsDto
                  {
                      MachineShiftSolutionId = t.Key.MachineShiftSolutionId,
                      MachineShiftSolutionName = t.Key.MachineShiftSolutionName
                  });
        }
        private List<string> GetUnionTables(GetDeviceGroupQualifiedRateRequestDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
            .Where(s => s.TargetTable == "TraceCatalogs").ToList()
            .Where(s => Convert.ToDateTime(input.StartTime) <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= Convert.ToDateTime(input.EndTime))
            .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
    }
}
