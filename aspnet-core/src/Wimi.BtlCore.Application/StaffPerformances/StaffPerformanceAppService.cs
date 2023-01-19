namespace Wimi.BtlCore.StaffPerformances
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
    using Abp.UI;

    using Authorization.Users;

    using AutoMapper;

    using Dto;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using StaffPerformance;
    using Wimi.BtlCore.Archives;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.BasicData.StateInfos;

    public class StaffPerformanceAppService : BtlCoreAppServiceBase, IStaffPerformanceAppService
    {
        private readonly IRepository<Machine> machineRepository;

        private readonly IStaffPerformaceRepository staffPerformaceRepository;

        private readonly IRepository<StateInfo> stateInfoRepository;

        private readonly IRepository<User, long> userRepository;

        private readonly IRepository<ShiftSolution> ssRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        public StaffPerformanceAppService(
            IRepository<Machine> machineRepository,
            IRepository<User, long> userRepository,
            IStaffPerformaceRepository staffPerformaceRepository,
            IRepository<StateInfo> stateInfoRepository,
            IRepository<ShiftSolution> ssRepository,
            IRepository<ArchiveEntry> archiveEntryRepository
            )
        {
            this.machineRepository = machineRepository;
            this.userRepository = userRepository;
            this.staffPerformaceRepository = staffPerformaceRepository;
            this.stateInfoRepository = stateInfoRepository;
            this.ssRepository = ssRepository;
            this.archiveEntryRepository = archiveEntryRepository;
        }

        /// <summary>
        /// 获取ScollTab显示数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<StaffPerformanceDto>> GetScrollTab(StaffPerformanceRequestDto input)
        {
            if (input.MachineIdList.Count == 0)
            {
                return new List<StaffPerformanceDto>();
            }

            IEnumerable<StaffPerformanceDto> result;

            switch (input.QueryType)
            {
                case EnumStaffPerformanceQueryType.ByStaff:
                    result = await this.GetStaffScrollTab(input);
                    break;
                case EnumStaffPerformanceQueryType.ByMachine:
                    result = await this.GetMachineScrollTab(input);
                    break;
                default:
                    throw new UserFriendlyException(this.L("MethodIsNotImplemented"));
            }

            return result;
        }

        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<StaffPerformanceDto>> GetStaffList()
        {
            return
                await
                (from u in this.userRepository.GetAll()
                 select new StaffPerformanceDto { UserId = u.Id, UserName = u.Name }).ToListAsync();
        }

        /// <summary>
        /// 获取班次方案列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<dynamic>> GetShiftSolutionList()
        {
            return
                await
                (from u in this.ssRepository.GetAll()
                 select new { ShiftSolutionId = u.Id, ShiftSolutionName = u.Name }).ToListAsync();
        }

        /// <summary>
        /// 获取所选时间内基础状态数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<StaffPerformanceDto> GetStaffPerformance(StaffPerformanceRequestDto input)
        {
            var result = new StaffPerformanceDto();

            if (input.MachineIdList.Count == 0)
            {
                return result;
            }

            if (input.MachineIdList.Contains(0))
            {
                var machineids = this.machineRepository.GetAll().Select(s => s.Id).ToList();

                input.MachineIdList.Clear();

                input.MachineIdList.AddRange(machineids);
            }

            input.UnionTables = this.GetUnionTables(input);
            // 获取输入查询条件内获取到符合要求的基础状态数据

            var query = await this.staffPerformaceRepository.GetStaffStatePerformance(ObjectMapper.Map<StaffPerformance>(input));

            var stateBasicData = ObjectMapper.Map<List<StaffPerformanceStateDto>>(query);

            // 处理基础数据,构造前台ECharts需要的数据结构
            result.StateRates = (from e in stateBasicData
                                 select new StaffPerformanceStateRateDto
                                 {
                                     MachineId = e.MachineId,
                                     MachineName = e.MachineName,
                                     MachinesShiftDetailId = e.MachinesShiftDetailId,
                                     UserId = e.UserId,
                                     UserName = e.UserName,
                                     SummaryDate = e.SummaryDate,
                                     ShiftDay = e.ShiftDay,
                                     IsByShift = e.IsByShift,
                                     StopDurationRate = e.TotalDuration == 0 ? 0 : e.StopDuration / e.TotalDuration * 100,
                                     RunDurationRate = e.TotalDuration == 0 ? 0 : e.RunDuration / e.TotalDuration * 100,
                                     FreeDurationRate = e.TotalDuration == 0 ? 0 : e.FreeDuration / e.TotalDuration * 100,
                                     OfflineDurationRate = e.TotalDuration == 0 ? 0 : e.OfflineDuration / e.TotalDuration * 100,
                                     DebugDurationRate = e.TotalDuration == 0 ? 0 : e.DebugDuration / e.TotalDuration * 100
                                 }).ToList();

            // 获取输入查询条件内获取到符合要求的基础原因数据
            var basicDataQuery = await this.staffPerformaceRepository.GetStaffReasonPerformance(ObjectMapper.Map<StaffPerformance>(input));

            var reasonBasicData = ObjectMapper.Map<List<StaffPerformanceStateDto>>(basicDataQuery);

            // 处理基础数据,构造前台ECharts需要的数据结构
            result.ReasonRates = (from e in reasonBasicData
                                  select new StaffPerformanceStateRateDto
                                  {
                                      MachineId = e.MachineId,
                                      MachineName = e.MachineName,
                                      MachinesShiftDetailId = e.MachinesShiftDetailId,
                                      UserId = e.UserId,
                                      UserName = e.UserName,
                                      SummaryDate = e.SummaryDate,
                                      ShiftDay = e.ShiftDay,
                                      IsByShift = e.IsByShift,
                                      Hexcode = e.Hexcode,
                                      ReasonCode = e.ReasonCode,
                                      ReasonName = e.ReasonName,
                                      ReasonDuration = e.ReasonDuration,
                                      TotalDuration = e.TotalDuration,
                                      ReasonRate = e.TotalDuration == 0 ? 0 : e.ReasonDuration / e.TotalDuration * 100
                                  }).ToList();

            result.ReasonRates = this.GenerateStaffReasonPerformanceResult(result).ToList();

            return result;
        }
        private List<string> GetUnionTables(StaffPerformanceRequestDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => input.StartTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= input.EndTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }

        /// <summary>
        /// 调整原因结果,补零,使ECahrts显示正确结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private IEnumerable<StaffPerformanceStateRateDto> GenerateStaffReasonPerformanceResult(
            StaffPerformanceDto result)
        {
            // 获取所有的原因Code
            var reasonCodes = from si in this.stateInfoRepository.GetAll()
                              where si.Type == EnumMachineStateType.Reason
                              select
                                  new StaffPerformanceStateRateDto { ReasonCode = si.Code, ReasonName = si.DisplayName };

            // 获取集合中所有的半Key(MachineId+UserId+SummaryDate)
            var halfKeys = from e in result.StateRates
                           group new { e.MachineId, e.UserId, e.SummaryDate } by
                               new { e.MachineId, e.UserId, e.SummaryDate }
                           into gd
                           select
                               new StaffPerformanceStateRateDto
                               {
                                   MachineId = gd.Key.MachineId,
                                   UserId = gd.Key.UserId,
                                   SummaryDate = gd.Key.SummaryDate
                               };

            // 根据reasonCodes和halfkeys组合成所有的全Key(MachineId+UserId+SummaryDate+ReasonCode)
            var keys = new List<StaffPerformanceStateRateDto>();

            var staffPerformanceStateRateDtos = halfKeys as StaffPerformanceStateRateDto[] ?? halfKeys.ToArray();

            foreach (var reasonCode in reasonCodes)
            {
                foreach (var halfKey in staffPerformanceStateRateDtos)
                {
                    var tempHalfKey = ObjectMapper.Map<StaffPerformanceStateRateDto>(halfKey);
                    tempHalfKey.ReasonCode = reasonCode.ReasonCode;
                    tempHalfKey.ReasonName = reasonCode.ReasonName;
                    keys.Add(tempHalfKey);
                }
            }

            // 比较Keys集合和返回结果集合,如果返回集合中不存在keys集合中的记录,进行补全
            var adjustedResult = new List<StaffPerformanceStateRateDto>();
            foreach (var key in keys)
            {
                var item =
                    result.ReasonRates.FirstOrDefault(
                        s =>
                        s.MachineId == key.MachineId && s.UserId == key.UserId && s.SummaryDate == key.SummaryDate
                        && s.ReasonCode == key.ReasonCode);

                if (item == null)
                {
                    key.ReasonRate = 0;
                    adjustedResult.Add(key);
                }
                else
                {
                    adjustedResult.Add(item);
                }
            }

            return adjustedResult;
        }

        /// <summary>
        /// 获取按设备分组的ScrollTab
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IEnumerable<StaffPerformanceDto>> GetMachineScrollTab(StaffPerformanceRequestDto input)
        {
            var query = from m in this.machineRepository.GetAll()
                        where input.MachineIdList.Contains(m.Id) || input.MachineIdList.Contains(0)
                        orderby m.SortSeq, m.Code
                        select new StaffPerformanceDto { MachineId = m.Id, MachineName = m.Name };
            return await query.ToListAsync();
        }

        /// <summary>
        /// 获取按人员分组ScrollTab
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<IEnumerable<StaffPerformanceDto>> GetStaffScrollTab(StaffPerformanceRequestDto input)
        {
            return await this.userRepository.GetAll().WhereIf(!input.UserIdList.Contains(0), s => input.UserIdList.Contains(s.Id)).Select(s => new StaffPerformanceDto { UserId = s.Id, UserName = s.Name }).ToListAsync();
        }
    }
}