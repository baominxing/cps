using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    public class CapacityRepository : ICapacityRepository
    {
        private readonly ISettingManager settingManager;
        private readonly IRepository<Machine> machineRepository;

        private readonly string connectionString;

        public CapacityRepository(ISettingManager settingManager,
             IRepository<Machine> machineRepository)
        {
            this.settingManager = settingManager;

            this.connectionString = AppSettings.Database.ConnectionString;
            this.machineRepository = machineRepository;
        }

        public async Task<IEnumerable<Yield4PerProgramOutputDto>> GetMachineAvgProgramDurationAndYield(int machineId, string startTime, string endTime)
        {
            var capacityFilter = Convert.ToBoolean(AppSettings.TraceabilityConfig.OfflineYield) ? "" : " AND c.IsLineOutput = 0 ";
            var sql = $@"SELECT c.MachineId,
                               c.ProgramName,
                               ISNULL(SUM(c.Yield),0) AS Yield,
                               ISNULL(AVG(c.Duration), 0)  AS AvgDuration
                        FROM Capacities       AS c
                        WHERE c.MachineId = @MachineId
                               AND c.StartTime BETWEEN @StartTime AND @EndTime  {capacityFilter}
                        GROUP BY c.MachineId, c.ProgramName
                        ORDER BY c.ProgramName";
 
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    MachineId = machineId,
                    StartTime = startTime,
                    EndTime = endTime
                };
                return (await conn.QueryAsync<Yield4PerProgramOutputDto>(sql, param)).ToList();
            }
        }

        public async Task<MachineYieldDto> GetMachineCapability(EnumStatisticalWays statisticalWay, EnumQueryMethod queryMethod,
            List<int> machineIdList, List<int> shiftSolutionIdList, List<int> deviceGroupIdList, string startTime, string endTime, List<string>unionTables)
        {
            var returnedValue = new MachineYieldDto();

            var groupBy = string.Empty;
            var orderBy = "t.SummaryDate  ";
            var typeBy = "m.SortSeq , m.Name ";

            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    groupBy = "scv.ShiftDayName ";
                    break;
                case EnumStatisticalWays.ByWeek:
                    groupBy = "scv.ShiftWeekName ";
                    break;
                case EnumStatisticalWays.ByMonth:
                    groupBy = "scv.ShiftMonthName ";
                    break;
                case EnumStatisticalWays.ByYear:
                    groupBy = "scv.ShiftYearName ";
                    break;
                case EnumStatisticalWays.ByShift:
                    groupBy = " scv.StartTime, scv.MachineShiftDetailName";
                    orderBy = " t.StartTime  ";
                    break;
            }

            string queryMethodSql = string.Empty;
            string selectSql = "";
            switch (queryMethod)
            {
                case EnumQueryMethod.ByMachine:
                    queryMethodSql = selectSql = "c.MachineId IN @MachineIdList ";
                    orderBy += " ,t.SortSeq ";
                    break;
                case EnumQueryMethod.ByGroup:
                    queryMethodSql = "mdg.DeviceGroupId IN @DeviceGroupIdList";
                    selectSql = " 1=1 ";
                    typeBy = "dg.DisplayName ";
                    break;
            }

            var whereMachineSql = machineIdList.Any() ? $" AND {queryMethodSql}  " : " ";
            whereMachineSql += shiftSolutionIdList.Any() ? " AND scv.ShiftSolutionId IN @ShiftSolutionIdList " : " ";

            var capacityFilter = Convert.ToBoolean(AppSettings.TraceabilityConfig.OfflineYield) ? "" : " AND c.IsLineOutput = 0 ";
             
            var unionQuery = $@"SELECT Yield,MachineId,MachinesShiftDetailId,IsLineOutput FROM dbo.Capacities as c WHERE {selectSql}
                                and ShiftDetail_ShiftDay between @StartTime  and @EndTime  and (Tag != 1 OR Tag is NULL) ";

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT Yield,MachineId,MachinesShiftDetailId,IsLineOutput FROM dbo.[{item}] as c WHERE {selectSql}
                                 and ShiftDetail_ShiftDay between @StartTime  and @EndTime  and (Tag != 1 OR Tag is NULL) ";
            }

            var executeSql = $@" select t.Yield, t.MachineName, t.SummaryDate from (
                        select isnull(sum(c.Yield),0) as Yield ,{typeBy} as MachineName, {groupBy} as SummaryDate
                        from ({unionQuery}) AS c
                        inner join Machines AS m  on ( m.id  = c.MachineId and m.IsDeleted = 0 and m.IsActive =1 )
                        inner join MachineDeviceGroups AS mdg on mdg.MachineId = m.Id
                        inner join DeviceGroups AS dg on dg.Id = mdg.DeviceGroupId
                        inner join ShiftCalendarsView AS scv on (c.MachinesShiftDetailId = scv.MachineShiftDetailId)
                        where  1=1 {whereMachineSql} {capacityFilter}
                        group by  {typeBy} , {groupBy} ) as T 
                 order by {orderBy}  ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new
                {
                    MachineIdList=machineIdList,
                    ShiftSolutionIdList= shiftSolutionIdList,
                    DeviceGroupIdList= deviceGroupIdList,
                    StartTime = startTime,
                    EndTime = endTime
                };

                var sqlData = await conn.QueryAsync<TableData>(executeSql, sqlParameters);
                var originalQueries = sqlData.ToList();

                var summaryDateQueries = from q in originalQueries
                                         group q.SummaryDate
                                            by q.SummaryDate into qg
                                         select new { SummaryDate = qg.Key };

                var machineQueries = from q in originalQueries
                                     group new { q.MachineName }
                                     by new { q.MachineName } into qg
                                     select new
                                     {
                                         MachineName = qg.Key.MachineName
                                     };

                var crossedQueries = from dt in summaryDateQueries
                                     from mpq in machineQueries
                                     select new
                                     {
                                         SummaryDate = dt.SummaryDate,
                                         MachineName = mpq.MachineName
                                     };

                var crossedResult = from cq in crossedQueries
                                    join oq in originalQueries on new { cq.SummaryDate, cq.MachineName } equals new { oq.SummaryDate, oq.MachineName } into leftQueries
                                    from lq in leftQueries.DefaultIfEmpty()
                                    select new TableData
                                    {
                                        SummaryDate = cq.SummaryDate,
                                        MachineName = cq.MachineName,
                                        Yield = lq == null ? 0 : lq.Yield
                                    };

                returnedValue.TableDataList = crossedResult.ToList();

                returnedValue.ChartDataList = (from cr in crossedResult
                                               group new { cr.MachineName, cr.Yield }
                                                  by new { cr.MachineName } into crg
                                               select new ChartData
                                               {
                                                   MachineName = crg.Key.MachineName,
                                                   Yields = crg.Select(s => s.Yield).ToList()
                                               }).ToList();

                return returnedValue;
            }
        }

        public string GetMaxCapacitySyncDateTime()
        {
            var sql = " SELECT MAX(c.MongoCreationTime) FROM Capacities AS c WHERE c.IsLineOutput = 0 ";

            var unionTabls = ListUnionTable();

            foreach (var item in unionTabls)
            {
                sql += $" UNION ALL SELECT MAX(c.MongoCreationTime) FROM [{item}] AS c WHERE c.IsLineOutput = 0 ";
            }

            using var conn = new SqlConnection(this.connectionString);

            var result = conn.Query<string>(sql).Max();

            return !result.IsNullOrWhiteSpace() ? result : string.Empty;

            List<string> ListUnionTable()
            {
                var executeSql = @"
SELECT DISTINCT
       ArchivedTable
FROM dbo.ArchiveEntries
WHERE TargetTable = N'Capacities'";

                using var conn = new SqlConnection(this.connectionString);

                var unionTables = conn.Query<string>(executeSql);

                return unionTables.ToList();
            }
        }

        public GetMachineStateRateInputDto GetStartTimeOfGanttChart(GetMachineStateRateInputDto input)
        {
            var returnValue = new GetMachineStateRateInputDto();

            //获取所有设备得班次方案列表，获取每个班次方案在当前工厂日得一个起始结束时间，得到最大时间范围
            string excutesql = @"select min(t.StartTime) as StartTime,max(t.EndTime) as EndTime
                   from   (
				   select
				   (case when IsNextDay=0 then DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate),StartTime) 
                         when IsNextDay=1 and  DATEDIFF(second,DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate)+1,EndTime),DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate)+1,StartTime))<0 then DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate)+1,StartTime)
				         else DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate),StartTime)  end
				   ) as StartTime,

                  (case when IsNextDay=1 then DATEADD(DAY,datediff(DAY,StartTime,@SummaryDate)+1,EndTime) else EndTime end) EndTime
                  from ShiftSolutionItems
                  where ShiftSolutionId in (
  	               select msd.ShiftSolutionId from Machines AS m
	                            INNER JOIN MachinesShiftDetails AS msd  ON  msd.MachineId = m.Id
                                INNER JOIN ShiftSolutionItems AS ssi  ON  msd.ShiftSolutionId = ssi.ShiftSolutionId
                                where m.IsDeleted=0 and msd.ShiftDay=@SummaryDate
	                            group by msd.ShiftSolutionId 
                              )) t";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    SummaryDate = Convert.ToDateTime(input.SummaryDate).ToString("yyyy-MM-dd HH:mm:ss")
                };
                returnValue = (conn.QueryFirst<GetMachineStateRateInputDto>(excutesql, param));

            }

            return returnValue;
        }

        public async Task<IEnumerable<MachineStateDto>> GetSummaryDate(GetMachineStateRateInputDto input)
        {
            // ReSharper disable once RedundantAssignment
            var showColumn = string.Empty;
            // ReSharper disable once RedundantAssignment
            var orderBy = string.Empty;
            var groupBy = string.Empty;
            switch (input.StatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                case EnumStatisticalWays.ByDay:
                    showColumn = " CONVERT(NVARCHAR,DATE,23) SummaryDate";
                    orderBy = "Order By DATE desc ";
                    break;
                case EnumStatisticalWays.ByWeek:
                    showColumn = "CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, ISOWeekOfYear) SummaryDate";
                    groupBy = "GROUP BY CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, ISOWeekOfYear)";
                    orderBy = "Order By CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, ISOWeekOfYear)";
                    break;
                case EnumStatisticalWays.ByMonth:
                    showColumn = "CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, MONTH) SummaryDate";
                    groupBy = "GROUP BY CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, MONTH)";
                    orderBy = "Order By CONVERT(NVARCHAR, Year) + ' - ' + CONVERT(NVARCHAR, MONTH)";
                    break;
                case EnumStatisticalWays.ByYear:
                    showColumn = "Year SummaryDate";
                    groupBy = "GROUP BY Year";
                    orderBy = "Order By Year";
                    break;
            }

            var executeSql = $@"SELECT {showColumn} 
                                    FROM Calendars WHERE DATE >=@StartTime AND DATE <=@EndTime
                                    {groupBy}
                                    {orderBy}";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new { input.StartTime, input.EndTime };

                return await conn.QueryAsync<MachineStateDto>(executeSql, sqlParameters);
            }
        }
    }
}
