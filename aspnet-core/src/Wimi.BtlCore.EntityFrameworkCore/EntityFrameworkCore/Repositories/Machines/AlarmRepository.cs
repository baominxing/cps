using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly ISettingManager settingManager;
        private readonly IMachineManager machineManager;
        private readonly ShiftCalendarManager shiftCalendarManager;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly string connectionString;

        public AlarmRepository(ISettingManager settingManager,
            IMachineManager machineManager,
            ShiftCalendarManager shiftCalendarManager,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.settingManager = settingManager;
            this.machineManager = machineManager;
            this.shiftCalendarManager = shiftCalendarManager;
            this.archiveEntryRepository= archiveEntryRepository;
            this.connectionString = AppSettings.Database.ConnectionString;
        }

        public async Task<IEnumerable<MachineAlarmStatistices>> GetAlarmChartData(GetMachineAlarms input)
        {
            var executeSql = this.GenerateGetAlarmChartDataSqlString(input);
            var param = new { input.StartTime, input.EndTime, input.MachineIdList, input.MachineShiftSolutionIdList };

            return await this.ExecuteForData<MachineAlarmStatistices>(executeSql, param);
        }

        public async Task<int> GetAlarmChartDataCount(GetMachineAlarms input)
        {
            var executeSql = this.GenerateGetAlarmChartDataCountSqlString(input);
            var param = new { input.StartTime, input.EndTime, input.MachineIdList, input.MachineShiftSolutionIdList };

            return await this.ExecuteForCount(executeSql, param);
        }

        /// <summary>
        ///     获取点击设备的报警信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MachineAlarmStatistices>> GetAlarmDetailData(GetMachineAlarms input)
        {
            var executeSql = this.GenerateGetAlarmDetailDataSqlString(input);
            var param = new { input.SummaryDate, input.MachineIdList };
            return await this.ExecuteForData<MachineAlarmStatistices>(executeSql, param);
        }

        /// <summary>
        ///     获取点击详细信息按钮弹出框数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MachineAlarmStatistices>> GetAlarmDetailDataForModal(GetMachineAlarms input)
        {
            var executeSql = this.GenerateGetAlarmDetailDataForModalSqlString(input);

            var param = new { input.MachineIdList, input.SummaryDate, input.AlarmCode };

            return await this.ExecuteForData<MachineAlarmStatistices>(executeSql, param);
        }

        /// <summary>
        ///     获取查询的设备信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MachineAlarmStatistices>> GetQueriedMachineInfo(GetMachineAlarms input)
        {
            var executeSql = this.GenerateGetQueriedMachineInfoSqlString(input);
            var param = new { input.StartTime, input.EndTime, input.MachineIdList, input.MachineShiftSolutionNameList };
            return await this.ExecuteForData<MachineAlarmStatistices>(executeSql, param);
        }

        public string GetMaxSyncAlarmDateTime()
        {
            var sql = " SELECT MAX(a.MongoCreationTime) FROM Alarms AS a ";

            var unionTabls = ListUnionTable();

            foreach (var item in unionTabls)
            {
                sql += $" UNION ALL SELECT MAX(a.MongoCreationTime) FROM [{item}] AS a ";
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
WHERE TargetTable = N'Alarms'";

                using var conn = new SqlConnection(this.connectionString);

                var unionTables = conn.Query<string>(executeSql);

                return unionTables.ToList();
            }
        }

        private async Task<int> ExecuteForCount(string executeSql, object parameters)
        {
            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var recordCount = await conn.ExecuteScalarAsync<int>(executeSql, parameters);

                return recordCount;
            }
        }

        private async Task<IEnumerable<T>> ExecuteForData<T>(string executeSql, object parameters)
            where T : class
        {
            IEnumerable<T> result;

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                result = await conn.QueryAsync<T>(executeSql, parameters);
            }

            return result;
        }

        /// <summary>
        ///     生成GetAlarmChartDataCount方法的SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateGetAlarmChartDataCountSqlString(GetMachineAlarms input)
        {
            EnumStatisticalWays enmuStatisticalWay;

            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
            {
                enmuStatisticalWay = EnumStatisticalWays.ByDay;
            }

            var selectString = string.Empty;
            var groupByString = string.Empty;
            var whereString = string.Empty;
            var calendarJoinString = string.Empty;

            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                    selectString = "scv.MachineShiftDetailName";
                    groupByString = "scv.MachineShiftDetailName";
                    whereString = "scv.ShiftSolutionId IN @MachineShiftSolutionIdList";
                    break;
                case EnumStatisticalWays.ByDay:
                    selectString = "scv.ShiftDayName";
                    groupByString = "scv.ShiftDayName";
                    whereString = "1 = 1";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectString = "scv.ShiftMonthName";
                    groupByString = "scv.ShiftMonthName";
                    whereString = "1 = 1";
                    break;
            }
            var unionQuery = $@"SELECT MachineId,MachinesShiftDetailId FROM dbo.Alarms WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT MachineId,MachinesShiftDetailId FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;
            }

            var executeSql = $@"
SELECT A.MachineId,
       a.MachineName,
       b.MachineGroupName,
       a.SummaryDate,
       a.AlarmCount
FROM   (
           SELECT scv.MachineId,
                  scv.MachineName,
                  {selectString}          SummaryDate,
                  COUNT(a.MachineId)      AlarmCount
           FROM   ({unionQuery})         AS a
                  INNER JOIN ShiftCalendarsView  AS scv
                       ON  a.MachinesShiftDetailId = scv.MachineShiftDetailId
           WHERE scv.ShiftDay BETWEEN @StartTime AND @EndTime And a.MachineId IN @MachineIdList AND {whereString}
           GROUP BY scv.MachineId,scv.MachineName,{groupByString}
       ) A
       JOIN (
                SELECT m2.Id MachineId,
                       MachineGroupName = STUFF(
                           (
                               SELECT ',' + dg.DisplayName
                               FROM   Machines AS m
                                      INNER JOIN MachineDeviceGroups AS mdg
                                           ON  mdg.MachineId = m.Id
                                      JOIN DeviceGroups AS dg
                                           ON  dg.Id = mdg.DeviceGroupId
                               WHERE  m.Id = m2.Id
                                      FOR XML PATH('')
                           ),
                           1,
                           1,
                           ''
                       )
                FROM   Machines AS m2
            ) B
            ON  a.MachineId = b.MachineId
";
            return executeSql;
        }

        /// <summary>
        ///     生成GetAlarmChartData方法的SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateGetAlarmChartDataSqlString(GetMachineAlarms input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            var selectString = string.Empty;
            var selectOrderByString = string.Empty;
            var groupByString = string.Empty;
            var whereString = string.Empty;
            var calendarJoinString = string.Empty;
            var orderByString = string.Empty;
            var shiftselect = "";
            var shiftchildselect = "";
            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                    selectString = "scv.MachineShiftDetailName";
                    selectOrderByString = "ShiftItemId,";
                    groupByString = "scv.ShiftItemId,scv.MachineShiftDetailName,scv.ShiftDay,scv.ShiftItemSeq";
                    whereString = "scv.ShiftSolutionId IN @MachineShiftSolutionIdList";
                    orderByString = "Order By A.ShiftItemId";
                    shiftchildselect = ", scv.ShiftDay , scv.ShiftItemSeq";
                    shiftselect = ", A.ShiftDay , A.ShiftItemSeq ";
                    break;
                case EnumStatisticalWays.ByDay:
                    selectString = "scv.ShiftDayName";
                    groupByString = "scv.ShiftDayName";
                    whereString = "1 = 1";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectString = "scv.ShiftMonthName";
                    groupByString = "scv.ShiftMonthName";
                    whereString = "1 = 1";
                    break;
            }
            var unionQuery = $@"SELECT MachineId,MachinesShiftDetailId FROM dbo.Alarms WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT MachineId,MachinesShiftDetailId FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            }

            var executeSql = $@"
SELECT {selectOrderByString} a.MachineId,a.MachineName,b.MachineGroupName,a.SummaryDate,a.AlarmCount {shiftselect}
FROM 
( 
    SELECT {selectOrderByString} scv.MachineId,scv.MachineName,{selectString} SummaryDate,COUNT(a.MachineId) AlarmCount {shiftchildselect}
    FROM ({unionQuery}) AS a INNER JOIN ShiftCalendarsView  AS scv ON  a.MachinesShiftDetailId = scv.MachineShiftDetailId
    WHERE scv.ShiftDay BETWEEN @StartTime AND @EndTime And a.MachineId IN @MachineIdList AND {whereString}
    GROUP BY scv.MachineId,scv.MachineName,{groupByString}
) A JOIN 
(
                SELECT m2.Id MachineId,
                       MachineGroupName = STUFF(
                           (
                               SELECT ',' + dg.DisplayName
                               FROM   Machines AS m
                                      INNER JOIN MachineDeviceGroups AS mdg
                                           ON  mdg.MachineId = m.Id
                                      JOIN DeviceGroups AS dg
                                           ON  dg.Id = mdg.DeviceGroupId
                               WHERE  m.Id = m2.Id
                                      FOR XML PATH('')
                           ),
                           1,
                           1,
                           ''
                       )
                FROM   Machines AS m2
            ) B
            ON  a.MachineId = b.MachineId
{orderByString}
";
            return executeSql;
        }

        /// <summary>
        ///     生成GetAlarmDetailDataForModal方法的SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateGetAlarmDetailDataForModalSqlString(GetMachineAlarms input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            var whereString = string.Empty;
            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                    whereString = "scv.MachineShiftDetailName";
                    break;
                case EnumStatisticalWays.ByDay:
                    whereString = "scv.ShiftDayName";
                    break;
                case EnumStatisticalWays.ByMonth:
                    whereString = "scv.ShiftMonthName";
                    break;
            }
            var unionQuery = $@"SELECT MachineId,StartTime,Code,Message,MachinesShiftDetailId FROM dbo.Alarms WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT MachineId,StartTime,Code,Message,MachinesShiftDetailId FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;
            }

            #region Component Designer generated code

            var executeSql = $@"
                    SELECT a.MachineId,
                           scv.MachineName,
                           a.StartTime,
                           a.Message AlarmMessage
                    FROM    ({unionQuery})          AS a
                           INNER JOIN ShiftCalendarsView  AS scv
                                ON  a.MachinesShiftDetailId = scv.MachineShiftDetailId
                    WHERE a.Code = @AlarmCode And a.MachineId IN @MachineIdList AND {whereString} = @SummaryDate
                    ORDER BY a.StartTime ";

            #endregion

            return executeSql;
        }

        /// <summary>
        ///     生成GetAlarmDetailData方法的SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateGetAlarmDetailDataSqlString(GetMachineAlarms input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;
            var selectString = string.Empty;
            var groupByString = string.Empty;
            var whereString = string.Empty;
            var calendarJoinString = string.Empty;
            switch (input.SelectString)
            {
                case "TOP10":
                    selectString = "TOP 10";
                    break;
                case "ALL": break;
            }

            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    selectString += "scv.ShiftDayName";
                    whereString = "scv.ShiftDayName";
                    groupByString = "scv.ShiftDayName";
                    break;
                case EnumStatisticalWays.ByShift:
                    selectString = "scv.MachineShiftDetailName";
                    whereString = "scv.MachineShiftDetailName";
                    groupByString = "scv.MachineShiftDetailName";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectString += "scv.ShiftMonthName";
                    whereString = "scv.ShiftMonthName";
                    groupByString = "scv.ShiftMonthName";
                    break;
            }
            var unionQuery = $@"SELECT MachineId,StartTime,Code,Message,MachinesShiftDetailId FROM dbo.Alarms WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT MachineId,StartTime,Code,Message,MachinesShiftDetailId FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;
            }

            var executeSql = $@"
SELECT 
{selectString} SummaryDate,
a.MachineId,
a.Code                   AlarmCode,
a.Message                AlarmMessage,
COUNT(a.MachineId)       AlarmCount
FROM    ({unionQuery})            AS a
INNER JOIN ShiftCalendarsView  AS scv
ON  a.MachinesShiftDetailId = scv.MachineShiftDetailId
WHERE a.MachineId IN @MachineIdList AND {whereString} = @SummaryDate
GROUP BY a.MachineId,a.Code,a.Message,{groupByString}
ORDER BY COUNT(a.MachineId) DESC
";
            return executeSql;
        }

        /// <summary>
        ///     生成GetQueriedMachineInfo方法的SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateGetQueriedMachineInfoSqlString(GetMachineAlarms input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            var whereString = string.Empty;
            var shiftJoinString = string.Empty;
            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                    shiftJoinString = @"
JOIN MachinesShiftDetails AS msd
ON  msd.MachineId = m2.Id
JOIN ShiftSolutions As ss 
ON ss.Id = msd.ShiftSolutionId";
                    whereString = " AND ss.Name IN @MachineShiftSolutionNameList"
                                  + " AND msd.ShiftDay BETWEEN @StartTime AND @EndTime";
                    break;
            }
            var executeSql = $@"
SELECT m2.Id       MachineId,
       m2.Name     MachineName,
       MachineGroupName = STUFF(
           (
               SELECT ',' + dg.DisplayName
               FROM   Machines           AS m
                      INNER JOIN MachineDeviceGroups AS mdg
                           ON  mdg.MachineId = m.Id
                      JOIN DeviceGroups  AS dg
                           ON  dg.Id = mdg.DeviceGroupId
               WHERE  m.Id = m2.Id
                      FOR XML PATH('')
           ),
           1,
           1,
           ''
       )
FROM   Machines AS m2
       {shiftJoinString}
WHERE  m2.Id IN @MachineIdList
       {whereString}
";
            return executeSql;
        }

        public async Task ImportDataByCover(ImportDataDto input)
        {
            // 1.创建临时表
            // Make a temp table in sql server that matches our production table
            var executeSql = @"DELETE FROM [AlarmInfos] Where MachineId=" + input.MachineId;

            // Connect to DB
             using (var con = new SqlConnection(this.connectionString))
            {
                con.Open();

                // Execute the command to make a temp table
                var cmd = new SqlCommand(executeSql, con);
                cmd.ExecuteNonQuery();

                var columnMappings = new List<SqlBulkCopyColumnMapping>();
                var code = new SqlBulkCopyColumnMapping("Code", "Code");
                columnMappings.Add(code);
                var message = new SqlBulkCopyColumnMapping("Message", "Message");
                columnMappings.Add(message);
                var creationTime = new SqlBulkCopyColumnMapping("CreationTime", "CreationTime");
                columnMappings.Add(creationTime);
                var creatorUserId = new SqlBulkCopyColumnMapping("CreatorUserId", "CreatorUserId");
                columnMappings.Add(creatorUserId);
                var machineTypeIdMapping = new SqlBulkCopyColumnMapping("MachineId", "MachineId");
                columnMappings.Add(machineTypeIdMapping);

                // 2.批量插入到临时表
                // BulkCopy the data in the DataTable to the temp table
                using (var bulk = new SqlBulkCopy(con))
                {
                    foreach (var item in columnMappings) bulk.ColumnMappings.Add(item);
                    bulk.DestinationTableName = "AlarmInfos";
                    await bulk.WriteToServerAsync(input.ImportData.AsDataTable());
                }

                await Task.FromResult((object)null);
            }
        }

        public async Task ImportDataByIncrement(ImportDataDto input)
        {
            // 1.创建临时表
            // Make a temp table in sql server that matches our production table
            var tmpTable = @"
                    IF OBJECT_ID('tempdb..#temp') IS NOT NULL
                        DROP TABLE #temp

                    CREATE TABLE #temp
                    (
	                    [Code] [nvarchar](40) NOT NULL,
	                    [Message] [nvarchar](4000) NULL,
	                    [CreationTime] [datetime] NOT NULL,
	                    [CreatorUserId] [bigint] NULL,
	                    [MachineId] [int] NOT NULL DEFAULT ((0))
                    )";

            // Connect to DB
            using (var con = new SqlConnection(this.connectionString))
            {
                con.Open();

                // Execute the command to make a temp table
                var cmd = new SqlCommand(tmpTable, con);
                cmd.ExecuteNonQuery();

                var columnMappings = new List<SqlBulkCopyColumnMapping>();
                var code = new SqlBulkCopyColumnMapping("Code", "Code");
                columnMappings.Add(code);
                var message = new SqlBulkCopyColumnMapping("Message", "Message");
                columnMappings.Add(message);
                var creationTime = new SqlBulkCopyColumnMapping("CreationTime", "CreationTime");
                columnMappings.Add(creationTime);
                var creatorUserId = new SqlBulkCopyColumnMapping("CreatorUserId", "CreatorUserId");
                columnMappings.Add(creatorUserId);
                var machineId = new SqlBulkCopyColumnMapping("MachineId", "MachineId");
                columnMappings.Add(machineId);

                // 2.批量插入到临时表
                // BulkCopy the data in the DataTable to the temp table
                using (var bulk = new SqlBulkCopy(con))
                {
                    foreach (var item in columnMappings) bulk.ColumnMappings.Add(item);
                    bulk.DestinationTableName = "#temp";
                    await bulk.WriteToServerAsync(input.ImportData.AsDataTable());
                }

                // 3.用merge命令执行upsert
                var mergeSql = @"
                        MERGE INTO AlarmInfos AS TARGET 
                        USING #temp AS Source 
                        ON TARGET.Code = Source.Code AND TARGET.MachineId = Source.MachineId
                        WHEN MATCHED THEN 
                        UPDATE  
                        SET TARGET.[Message] = Source.[Message]
                        WHEN NOT MATCHED THEN INSERT  
                        ( 
                            [Code] 
                           ,[Message] 
                           ,[CreationTime] 
                           ,[CreatorUserId] 
                           ,[MachineId] 
                        ) 
                        VALUES 
                        ( 
                            Source.[Code] 
                           ,Source.[Message] 
                           ,Source.[CreationTime] 
                           ,Source.[CreatorUserId] 
                           ,Source.[MachineId] 
                        );";
                cmd.CommandText = mergeSql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<AlarmExportDto>> QueryAlarmExportData(string statisticalWayStr,DateTime? startTime,DateTime? endTime, List<int> machineIdList, List<int> machineShiftSolutionIdList)
        {
            if (!Enum.TryParse(statisticalWayStr, out EnumStatisticalWays statisticalWay))
            {
                statisticalWay = EnumStatisticalWays.ByDay;
            }

             var  newStartTime =startTime ?? DateTime.Today;
              var newEndTime = endTime ?? DateTime.Today;

            var newMachineListId = (await this.machineManager.ListMachines()).Select(t => t.Value).ToList();

            if (!machineIdList.Any())
            {
                machineIdList = newMachineListId;
            }

            var correctedDateList = (await this.shiftCalendarManager.CorrectQueryDate(newStartTime, newEndTime, statisticalWay, machineIdList, machineShiftSolutionIdList)).ToList();

            if (!correctedDateList.Any())
            {
                return   null;
            }

            var stDate = correctedDateList.First().ShiftDay;
            var edDate = correctedDateList.Last().ShiftDay;

            //根据查询时间范围，获取需要union的分表
            var unionTables = this.GetUnionTables(Convert.ToDateTime(stDate), Convert.ToDateTime(edDate));

            var unionQuery = $@"SELECT * FROM dbo.Alarms WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT * FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;
            }

            var groupBy = "T.ShiftDayName";
            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    break;
                case EnumStatisticalWays.ByWeek:
                    groupBy = "T.ShiftWeekName ";
                    break;
                case EnumStatisticalWays.ByMonth:
                    groupBy = "T.ShiftMonthName ";
                    break;
                case EnumStatisticalWays.ByYear:
                    groupBy = "T.ShiftYearName ";
                    break;
                case EnumStatisticalWays.ByShift:
                case EnumStatisticalWays.ByMachineShiftDetail:
                    groupBy = "T.MachineShiftDetailName";
                    break;
            }

            var commonSql = $@" select s.MachineShiftDetailName,s.ShiftDay,s.ShiftDayName,s.ShiftWeekName,s.ShiftMonthName,s.ShiftYearName, s.MachineName, a.MachineCode, a.MachineId, a.Code ,a.Message from  ({unionQuery}) as a 
                                        inner join ShiftCalendarsView as s on a.MachinesShiftDetailId = s.MachineShiftDetailId ";
            var codeCountSql = $@"select {groupBy} as SummaryDate ,  T.MachineCode,T.MachineName, T.Code, T.Message,count(T.Code) Count from ( {commonSql} ) as T
                                  where t.ShiftDay between  @startTime and @endTime  and t.MachineId in @MachineIdList
                                 group by {groupBy}, T.MachineCode, T.MachineName, T.Code, T.Message ";

            var totalCountSql = $@"select {groupBy} as SummaryDate , T.MachineCode, count(T.Code) TotalCount from ( {commonSql}) as T
                                  where t.ShiftDay between  @startTime and @endTime and t.MachineId in @MachineIdList
                                  group by {groupBy},T.MachineCode ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    startTime = stDate,
                    endTime = edDate,
                    MachineIdList = machineIdList
                };

                var countQuery = await conn.QueryAsync<AlarmExportDto>(codeCountSql, param);
                var totalQuery = await conn.QueryAsync<AlarmExportDto>(totalCountSql, param);

                var query = countQuery
                   .Join(totalQuery, c => new { c.SummaryDate, c.MachineCode }, t => new { t.SummaryDate, t.MachineCode },
                        (c, t) => new { c, t }).Select(s => new AlarmExportDto
                        {
                            SummaryDate = s.c.SummaryDate,
                            Count = s.c.Count,
                            Code = s.c.Code,
                            MachineCode = s.c.MachineCode,
                            MachineName = s.c.MachineName,
                            Message = s.c.Message,
                            TotalCount = s.t.TotalCount
                        }).OrderBy(t => t.SummaryDate).ThenBy(t => t.MachineCode).ThenBy(t => t.Rate);

                return query;
            }
        }

        public List<string> GetUnionTables(DateTime startTime,DateTime endTime)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "Alarms").ToList()
                .Where(s => startTime <= Convert.ToDateTime(s.ArchiveValue).Date
                && Convert.ToDateTime(s.ArchiveValue).Date <= endTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
    }
}