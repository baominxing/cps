using Abp.Configuration;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis
{
    public class ReasonFeedbackAnalysisAppService : BtlCoreAppServiceBase, IReasonFeedbackAnalysisAppService
    {
        private readonly IRepository<ReasonFeedbackRecord> reasonFeedbackRecordRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly ISettingManager settingManager;
        private readonly ICommonRepository commonRepository;

        public ReasonFeedbackAnalysisAppService(
            IRepository<ReasonFeedbackRecord> reasonFeedbackRecordRepository,
             IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
             IRepository<Machine> machineRepository,
             IRepository<StateInfo> stateInfoRepository,
             ISettingManager settingManager,
             ICommonRepository commonRepository
            )
        {
            this.reasonFeedbackRecordRepository = reasonFeedbackRecordRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineRepository = machineRepository;
            this.stateInfoRepository = stateInfoRepository;
            this.settingManager = settingManager;
            this.commonRepository = commonRepository;
        }

        [HttpPost]
        public ReasonFeedBackAnalysisDto GetReasonFeedBackResult(ReasonFeedBackAnalysisRequestDto input)
        {
            if (input.DeviceGroupId == 0)
            {
                return new ReasonFeedBackAnalysisDto();
            }

            var endTime = input.EndTime.AddDays(1);

            var queryMachine = (from m in machineRepository.GetAll()
                                join mdg in machineDeviceGroupRepository.GetAll() on m.Id equals mdg.MachineId
                                where mdg.DeviceGroupId == input.DeviceGroupId
                                select new { m.Id, m.Name }).OrderBy(m => m.Id).ToList();

            var summaryDateList = commonRepository.GetSummaryDateList(input.StartTime, endTime);

            var chartResult = new List<AnalysisChartData>();

            var machineNameList = new List<string>();

            int i = 0;
            foreach (var machine in queryMachine)
            {
                machineNameList.Add(machine.Name);
                for (int j = 0; j <= summaryDateList.Count() - 1; j++)
                {
                    var chartRecord = new AnalysisChartData();
                    var summaryDate = summaryDateList[j];
                    chartRecord.MachineName = machine.Name;
                    chartRecord.SummaryDate = summaryDate;
                    chartRecord.Times = GetReasonFeedBackTimes(machine.Id, summaryDate);
                    chartRecord.Duration = GetReasonFeedBackDuration(machine.Id, summaryDate);
                    chartRecord.HorizontalValue = j;
                    chartRecord.VerticalValue = i;

                    chartResult.Add(chartRecord);
                }

                i++;
            }

            var result = new ReasonFeedBackAnalysisDto
            {
                ChartData = chartResult
            };

            var query = (from rfr in reasonFeedbackRecordRepository.GetAll()
                         join mdg in machineDeviceGroupRepository.GetAll() on rfr.MachineId equals mdg.MachineId
                         join si in stateInfoRepository.GetAll() on rfr.StateCode equals si.Code
                         where mdg.DeviceGroupId == input.DeviceGroupId
                         && rfr.EndTime != null
                         && rfr.StartTime >= input.StartTime
                         && rfr.StartTime <= endTime
                         select new
                         {
                             SummaryDate = rfr.StartTime.Date,
                             rfr.Duration,
                             si.DisplayName,
                             si.Code
                         }).ToList();

            var countQuery = (query.GroupBy(q => new { q.SummaryDate, q.Code, q.DisplayName })
                .Select(g => new
                {
                    g.Key.DisplayName,
                    g.Key.SummaryDate,
                    g.Key.Code,
                    Count = g.Count()
                })).ToList();

            var sumQuery = (query.GroupBy(q => new { q.SummaryDate, q.Code, q.DisplayName })
               .Select(g => new
               {
                   g.Key.DisplayName,
                   g.Key.SummaryDate,
                   g.Key.Code,
                   SumDuration = g.Sum(s => s.Duration)
               })).ToList();

            var tableData = new List<AnalysisTableData>();

            foreach (var item in countQuery)
            {
                var tableRecord = new AnalysisTableData
                {
                    FeedBackReason = item.DisplayName,
                    StateCode = item.Code,
                    SummaryDate = item.SummaryDate.ToString(),
                    Times = item.Count,
                    Duration = sumQuery.Where(s => s.SummaryDate == item.SummaryDate && s.Code == item.Code).FirstOrDefault().SumDuration
                };
                tableData.Add(tableRecord);
            }

            result.TableData = tableData;

            return result;
        }

        [HttpPost]
        public List<ReasonFeedBackAnalysisDetailDto> GetDetail(ReasonFeedBackAnalysisDetailRequestDto input)
        {
            var endTime = input.SummaryDate.AddDays(1);

            var query = (from rfr in reasonFeedbackRecordRepository.GetAll()
                         join mdg in machineDeviceGroupRepository.GetAll() on rfr.MachineId equals mdg.MachineId
                         join si in stateInfoRepository.GetAll() on rfr.StateCode equals si.Code
                         join m in machineRepository.GetAll() on rfr.MachineId equals m.Id
                         where mdg.DeviceGroupId == input.DeviceGroupId
                         && si.Code == input.StateCode
                         && rfr.StartTime >= input.SummaryDate
                         && rfr.StartTime < endTime
                         select new ReasonFeedBackAnalysisDetailDto()
                         {
                             MachineId = m.Id,
                             MachineName = m.Name,
                             FeedBackReason = si.DisplayName,
                             Times = 1,
                             Duration = rfr.Duration
                         }).ToList();

            var result = (query.GroupBy(q => new { q.MachineId, q.MachineName, q.FeedBackReason })
                .Select(g => new ReasonFeedBackAnalysisDetailDto()
                {
                    MachineId = g.Key.MachineId,
                    MachineName = g.Key.MachineName,
                    FeedBackReason = g.Key.FeedBackReason,
                    Times = g.Sum(s => s.Times),
                    Duration = g.Sum(s => s.Duration)
                })).ToList();

            return result;
        }

        private int GetReasonFeedBackTimes(int machineId, DateTime summaryDate)
        {
            var endTime = summaryDate.AddDays(1);
            var query = reasonFeedbackRecordRepository.GetAll()
                .Where(s => s.MachineId == machineId
                && s.StartTime >= summaryDate
                && s.StartTime < endTime
                && s.EndTime != null).Count();

            return query;
        }

        private decimal GetReasonFeedBackDuration(int machineId, DateTime summaryDate)
        {
            var endTime = summaryDate.AddDays(1);
            var query = reasonFeedbackRecordRepository.GetAll()
                .Where(s => s.MachineId == machineId
                && s.StartTime >= summaryDate
                && s.StartTime < endTime
                && s.EndTime != null).ToList();

            if (query.Count == 0)
                return 0;
            else
                return query.Sum(s => s.Duration);
        }
    }
}
