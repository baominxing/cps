using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Trace;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Trace.Repository;
using Wimi.BtlCore.Trace.Repository.Dtos;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Trace
{
    public class TraceRepository : ITraceRepository
    {
        public async Task<PartDetail> QueryPartDetail(string partNo, string archivedTable)
        {
            var table = string.IsNullOrEmpty(archivedTable) ? $"TraceCatalogs" : archivedTable;

            var executeSql = $@"
SELECT catalog.PartNo,
       catalog.IsReworkPart,
       catalog.OfflineTime,
       catalog.OnlineTime,
       catalog.Qualified,
       shiftSolutionItem.Name ShiftName
FROM dbo.[{table}] AS catalog
    LEFT JOIN dbo.MachinesShiftDetails AS shiftDetail
        ON catalog.MachineShiftDetailId = shiftDetail.Id
    LEFT JOIN dbo.ShiftSolutionItems AS shiftSolutionItem
        ON shiftDetail.ShiftSolutionItemId = shiftSolutionItem.Id
WHERE catalog.PartNo = @PartNo" + Environment.NewLine;

            var parameters = new { PartNo = partNo };
            var partDetail = new PartDetail();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                partDetail = await conn.QueryFirstOrDefaultAsync<PartDetail>(executeSql, parameters);
            }

            return partDetail;
        }

        public async Task<List<TraceCatalogDto>> QueryTraceCatalog(QueryTraceCatalogInputDto input, int skipCount,int length)
        {
            var groupbySql = GetQueryTraceCatalogSql(input);
            var querySql = $"select t.* from ({groupbySql}) as t order by t.Id desc offset {skipCount} rows fetch next {length} rows only ";
            var query = new List<TraceCatalogDto>();
            var parameters = new { MachineIdList = input.MachineId, ShiftSolutionItemId = input.ShiftSolutionItemId, Station = input.StationCode };
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var sqlResult = await conn.QueryAsync<TraceCatalogDto>(querySql, parameters);

                if (sqlResult.Any())
                {
                    query = sqlResult.ToList();
                }
            }

            return query;
        }

        public async Task<int> QueryTraceCatalogCount(QueryTraceCatalogInputDto input)
        {
            var groupbySql = GetQueryTraceCatalogSql(input);
            var countSql = $"select count(*) from ({groupbySql}) as t";
            var count = 0;
            var parameters = new { MachineIdList = input.MachineId, ShiftSolutionItemId = input.ShiftSolutionItemId, Station = input.StationCode };
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                count =await conn.QueryFirstAsync<int>(countSql, parameters);
             }

            return count;
        }

        private string GetQueryTraceCatalogSql(QueryTraceCatalogInputDto input)
        {
            var groupbySql = string.Empty;
 
            var where = $"WHERE 1 = 1 ";
            var item1 = new List<string>();
            var item2 = new List<string>();

            if (input.EndDate == "") { input.EndDate = null; input.EndFirstTime = null; input.EndLastTime = null; }

            if (input.StartDate == "") { input.StartDate = null; input.StartFirstTime = null; input.StartLastTime = null; }

            if (!string.IsNullOrEmpty(input.PartNo))
            {
                where += $" AND tc.PartNo LIKE '%{input.PartNo.Trim()}%'";

                var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, input.PartNo);

                item1.AddRange(tables.Item1);
                item2.AddRange(tables.Item2);
            }
            else
            {
                var startTime = input.MachineId.Any() == false ? "tc.OnlineTime" : "tfr.EntryTime";
                var endTime = input.MachineId.Any() == false ? "tc.OfflineTime" : "tfr.LeftTime";

                if (!string.IsNullOrEmpty(input.StartDate))
                {
                    where += $" AND {startTime} BETWEEN '{input.StartFirstTime.Value:yyyy-MM-dd HH:mm:ss}' AND '{input.StartLastTime.Value:yyyy-MM-dd HH:mm:ss}'";

                    var tables =   this.GetUnionTables(input.StartFirstTime.Value.Date, input.StartLastTime.Value.Date, input.PartNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (!string.IsNullOrEmpty(input.EndDate))
                {
                    where += $" AND {endTime} BETWEEN '{input.EndFirstTime.Value:yyyy-MM-dd HH:mm:ss}' AND '{input.EndLastTime.Value:yyyy-MM-dd HH:mm:ss}'";

                    var tables =   this.GetUnionTables(input.EndFirstTime.Value.Date, input.EndLastTime.Value.Date, input.PartNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (string.IsNullOrEmpty(input.StartDate) && string.IsNullOrEmpty(input.EndDate))
                {
                    var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, "-");//给随便一个工件值，走全部分表查询逻辑

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }
            }

            if (input.DeviceGroupId > 0)
            {
                where += $" AND tc.DeviceGroupId = {input.DeviceGroupId} ";
            }

            if (input.MachineId.Any())
            {
                where += $" AND tfr.MachineId IN @MachineIdList ";
            }

            if (input.ShiftSolutionItemId > 0)
            {
                where += $" AND msd.ShiftSolutionItemId = @ShiftSolutionItemId ";
            }

            if (!string.IsNullOrEmpty(input.StationCode))
            {
                where += $" AND tfr.Station = @Station ";
            }

            var unionTables = Tuple.Create(item1.Distinct().ToList(), item2.Distinct().ToList());

            where += input.NgPartCatlogId == 0 ? "" : $" AND tc.Id = {input.NgPartCatlogId} ";

            var innerSql = $@"
SELECT tc.Id,
       tc.PartNo,
       tc.OfflineTime,
       tc.Qualified,
       tc.ArchivedTable
FROM dbo.TraceCatalogs AS tc
    JOIN dbo.TraceFlowRecords AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
{where}" + Environment.NewLine;

            for (int i = 0; i < unionTables.Item1.Count; i++)
            {
                //查询的数据涉及到分表
                innerSql += $@"
UNION ALL
SELECT tc.Id,
       tc.PartNo,
       tc.OfflineTime,
       tc.Qualified,
       tc.ArchivedTable
FROM dbo.[{unionTables.Item1[i]}] AS tc
    JOIN dbo.[{unionTables.Item2[i]}] AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
{where}" + Environment.NewLine;
            }

            groupbySql = @$"SELECT * FROM ( SELECT t1.*,row_number() OVER (partition BY t1.Id ORDER BY t1.Id DESC) AS rt FROM ({innerSql}) AS t1) AS t1 WHERE t1.rt = 1";
           
            return groupbySql;
        }

        public async Task<List<TraceFlowRecord>> QueryTraceFlowRecord(string partNo,string archivedTable)
        {
            var traceFlowRecordsRepository = new List<TraceFlowRecord>();

            var table = string.IsNullOrEmpty(archivedTable) ? $"TraceCatalogs" : archivedTable;
            var recordTable = table.Replace("TraceCatalogs", "TraceFlowRecords");
            var selectRecordTable = recordTable.Equals("TraceFlowRecords") ? "TraceFlowRecords"
    : $"(SELECT * FROM TraceFlowRecords UNION SELECT * FROM [{recordTable}])";
            var parameters = new { PartNo = partNo };

          var   executeSql = $@"
SELECT [Id],
       [PartNo],
       [FlowCode],
       [FlowDisplayName],
       [TraceFlowSettingId],
       [MachineCode],
       [MachineId],
       [Station],
       [EntryTime],
       [LeftTime],
       [State],
       [Tag],
       [ExtensionData],
       [UserId]
FROM {selectRecordTable} TFR
WHERE PartNo = @PartNo" + Environment.NewLine;

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                traceFlowRecordsRepository = (await conn.QueryAsync<TraceFlowRecord>(executeSql, parameters)).ToList();
            }
            return traceFlowRecordsRepository;
        }

        private Tuple<List<string>, List<string>> GetUnionTables(DateTime? startTime, DateTime? endTime, string partNo)
        {
            var traceCatalogsTables = new List<string>();
            var traceFlowRecordTables = new List<string>();

            var sql = string.Empty;

            //如果有输入工件序列号条件，需要忽略时间条件，遍历全部的表
            if (!string.IsNullOrEmpty(partNo))
            {

                sql = @"Select Distinct  ArchivedTable 
From ArchiveEntries
Where TargetTable ='TraceCatalogs' ";

            }
            else if (startTime.HasValue && endTime.HasValue)
            {

                sql = @"Select Distinct  ArchivedTable 
From ArchiveEntries
Where TargetTable ='TraceCatalogs' 
And  ArchiveValue Between @StartTime And @EndTime";

            }
            else if (startTime.HasValue && !endTime.HasValue)
            {

                sql = @"Select Distinct  ArchivedTable 
From ArchiveEntries
Where TargetTable ='TraceCatalogs' 
And  ArchiveValue >= @StartTime ";

            }
            else if (!startTime.HasValue && endTime.HasValue)
            {
                sql = @"Select Distinct  ArchivedTable 
From ArchiveEntries
Where TargetTable ='TraceCatalogs' 
And  ArchiveValue <= @EndTime ";
            }

            var param = new
            {
                StartTime = startTime.HasValue ? startTime.Value.ToString("yyyy-MM-dd") : new DateTime().ToString("yyyy-MM-dd"),
                EndTime = endTime.HasValue ? endTime.Value.ToString("yyyy-MM-dd") : new DateTime().ToString("yyyy-MM-dd"),
            };
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var sqlResult = conn.Query<string>(sql, param);

                if (sqlResult.Any())
                {
                    traceCatalogsTables = sqlResult.ToList();
                }
            }

            traceFlowRecordTables = traceCatalogsTables.Select(s => s.Replace("TraceCatalogs", "TraceFlowRecords")).ToList();

            var tuple = Tuple.Create(traceCatalogsTables, traceFlowRecordTables);

            return tuple;
        }

        public async Task<IEnumerable<TraceExportItem>> ListTraceExportItem(QueryTraceCatalogInputDto input)
        {
            var where = $" WHERE 1 = 1 ";
            var item1 = new List<string>();
            var item2 = new List<string>();

            if (input.EndDate == "") { input.EndDate = null; input.EndFirstTime = null; input.EndLastTime = null; }

            if (input.StartDate == "") { input.StartDate = null; input.StartFirstTime = null; input.StartLastTime = null; }

            if (!string.IsNullOrEmpty(input.PartNo))
            {
                where += $" AND tc.PartNo LIKE '%{input.PartNo.Trim()}%'";

                var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, input.PartNo);

                item1.AddRange(tables.Item1);
                item2.AddRange(tables.Item2);
            }
            else
            {
                var startTime = input.MachineId.Any() == false ? "tc.OnlineTime" : "tfr.EntryTime";
                var endTime = input.MachineId.Any() == false ? "tc.OfflineTime" : "tfr.LeftTime";

                if (!string.IsNullOrEmpty(input.StartDate))
                {
                    where += $" AND {startTime} BETWEEN '{input.StartFirstTime.Value:yyyy-MM-dd HH:mm:ss}' AND '{input.StartLastTime.Value:yyyy-MM-dd HH:mm:ss}'";

                    var tables =   this.GetUnionTables(input.StartFirstTime.Value.Date, input.StartLastTime.Value.Date, input.PartNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (!string.IsNullOrEmpty(input.EndDate))
                {
                    where += $" AND {endTime} BETWEEN '{input.EndFirstTime.Value:yyyy-MM-dd HH:mm:ss}' AND '{input.EndLastTime.Value:yyyy-MM-dd HH:mm:ss}'";

                    var tables =   this.GetUnionTables(input.EndFirstTime.Value.Date, input.EndLastTime.Value.Date, input.PartNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (string.IsNullOrEmpty(input.StartDate) && string.IsNullOrEmpty(input.EndDate))
                {
                    var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, "-");//给随便一个工件值，走全部分表查询逻辑

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }
            }

            if (input.DeviceGroupId > 0)
            {
                where += $" AND tc.DeviceGroupId = {input.DeviceGroupId} ";
            }

            if (input.MachineId.Any())
            {
                where += $" AND tfr.MachineId IN @MachineIdList ";
            }

            if (input.ShiftSolutionItemId > 0)
            {
                where += $" AND msd.ShiftSolutionItemId = @ShiftSolutionItemId ";
            }

            if (!string.IsNullOrEmpty(input.StationCode))
            {
                where += $" AND tfr.Station = @Station ";
            }

            var unionTables = Tuple.Create(item1.Distinct().ToList(), item2.Distinct().ToList());

            where += input.NgPartCatlogId == 0 ? "" : $" AND tc.Id = {input.NgPartCatlogId} ";

            var sql = $@"SELECT DISTINCT tc.PartNo,
                                   CASE
                                       WHEN tc.OfflineTime IS NULL THEN
                                           '未下线'
                                       ELSE
                                           '已下线'
                                   END AS TraceStates,
                                   tc.Qualified,
                                   sst.Name AS ShiftItemName,
                                   m.Name AS MachineName,
                                   tfs.DisplayName AS FlowName,
                                   tfr.EntryTime,
                                   tfr.LeftTime,
                                   tfr.State
                            FROM dbo.TraceCatalogs AS tc
                                INNER JOIN dbo.TraceFlowRecords AS tfr  ON tfr.PartNo = tc.PartNo
                                INNER JOIN dbo.MachinesShiftDetails AS msd ON msd.Id = tc.MachineShiftDetailId
                                INNER JOIN dbo.ShiftSolutionItems AS sst ON sst.Id = msd.ShiftSolutionItemId
                                INNER JOIN dbo.Machines AS m ON tfr.MachineId = m.Id
                                INNER JOIN dbo.TraceFlowSettings AS tfs ON tfs.Id = tfr.TraceFlowSettingId And tfs.DeviceGroupId = tc.DeviceGroupId
                            {where} " + Environment.NewLine;

            for (int i = 0; i < unionTables.Item1.Count; i++)
            {
                //查询的数据涉及到分表
                sql += $@"
UNION ALL
SELECT tc.PartNo,
                                   CASE
                                       WHEN tc.OfflineTime IS NULL THEN
                                           '未下线'
                                       ELSE
                                           '已下线'
                                   END AS TraceStates,
                                   tc.Qualified,
                                   sst.Name AS ShiftItemName,
                                   m.Name AS MachineName,
                                   tfs.DisplayName AS FlowName,
                                   tfr.EntryTime,
                                   tfr.LeftTime,
                                   tfr.State
                            FROM dbo.[{unionTables.Item1[i]}] AS tc
                                INNER JOIN dbo.[{unionTables.Item2[i]}] AS tfr  ON tfr.PartNo = tc.PartNo
                                INNER JOIN dbo.MachinesShiftDetails AS msd ON msd.Id = tc.MachineShiftDetailId
                                INNER JOIN dbo.ShiftSolutionItems AS sst ON sst.Id = msd.ShiftSolutionItemId
                                INNER JOIN dbo.Machines AS m ON tfr.MachineId = m.Id
                                INNER JOIN dbo.TraceFlowSettings AS tfs ON tfs.Id = tfr.TraceFlowSettingId And tfs.DeviceGroupId = tc.DeviceGroupId
                            {where}" + Environment.NewLine;
            }

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                return await conn.QueryAsync<TraceExportItem>(sql);
            }
        }

        public async Task<List<NgPartsResultDto>> QueryNgPart(string partNo, DateTime? startTime, DateTime? endTime,int length, int skipCount)
        {
            var query = new List<NgPartsResultDto>();
            var executeSql = GetQueryNgPartSql(partNo, startTime,endTime);
            var finalSql = $"SELECT Top({length})* FROM (SELECT *,ROW_NUMBER()over(order by id) as row_num from ({executeSql}) as temp )" + $"as temp2 WHERE row_num > {skipCount}";
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                query = (await conn.QueryAsync<NgPartsResultDto>(finalSql)).ToList();
            }
            return query;
        }

        public async Task<int> QueryNgPartCount(string partNo, DateTime? startTime, DateTime? endTime)
        {
            var totalCount = 0;
            var executeSql = GetQueryNgPartSql(partNo, startTime, endTime);
            var finalSql = $"select count(*) from ({executeSql}) as a";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                totalCount = await conn.QueryFirstAsync<int>(finalSql);
            }

            return totalCount;
        }

        public string GetQueryNgPartSql(string partNo,DateTime? startTime,DateTime? endTime)
        {
      
            var where = string.Empty;
            var item1 = new List<string>();
            var item2 = new List<string>();

            if (!string.IsNullOrEmpty(partNo))
            {
                where += $" AND tc.PartNo LIKE '%{partNo.Trim()}%'";

                var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, partNo);

                item1.AddRange(tables.Item1);
                item2.AddRange(tables.Item2);
            }
            else
            {
                if (startTime.HasValue)
                {
                    where += $" AND '{startTime.Value:yyyy-MM-dd HH:mm:ss}' <= tc.OnlineTime";

                    var tables =   this.GetUnionTables(startTime.Value.Date, null, partNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (endTime.HasValue)
                {
                    where += $" AND tc.OnlineTime <= '{endTime.Value:yyyy-MM-dd HH:mm:ss}'";

                    var tables =   this.GetUnionTables(null, endTime.Value.Date, partNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (!startTime.HasValue && !endTime.HasValue)
                {
                    var tables =   this.GetUnionTables(null, null, "-");//给随便一个工件值，走全部分表查询逻辑

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }
            }

            var unionTables = Tuple.Create(item1.Distinct().ToList(), item2.Distinct().ToList());

            var executeSql = $@"
SELECT tc.Id,
       tc.DeviceGroupId,
       dg.DisplayName DeviceGroupName,
       tc.PartNo,
       tc.OnlineTime,
       tc.OfflineTime,
       ssi.Id ShiftSolutionItemId,
       ss.Name + '-' + ssi.Name ShiftName,
       CASE
           WHEN tc.OfflineTime IS NULL THEN
               '未下线'
           ELSE
               '已下线'
       END State,
       tfr.MachineId,
       m.Name MachineName,
       tfr.Station StationCode,
       tfr.Station StationName
FROM dbo.TraceCatalogs AS tc
    JOIN dbo.TraceFlowRecords AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.Machines AS m
        ON tfr.MachineId = m.Id AND m.IsDeleted = 0
    LEFT JOIN dbo.DeviceGroups AS dg
        ON tc.DeviceGroupId = dg.Id
    LEFT JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
    LEFT JOIN dbo.ShiftSolutions AS ss
        ON msd.ShiftSolutionId = ss.Id
    LEFT JOIN dbo.ShiftSolutionItems AS ssi
        ON msd.ShiftSolutionItemId = ssi.Id
WHERE tc.Qualified != 1
      AND tfr.Tag = 2
{where}" + Environment.NewLine;

            for (int i = 0; i < unionTables.Item1.Count; i++)
            {
                //查询的数据涉及到分表
                executeSql += $@"
UNION ALL
SELECT tc.Id,
       tc.DeviceGroupId,
       dg.DisplayName DeviceGroupName,
       tc.PartNo,
       tc.OnlineTime,
       tc.OfflineTime,
       ssi.Id ShiftSolutionItemId,
       ss.Name + '-' + ssi.Name ShiftName,
       CASE
           WHEN tc.OfflineTime IS NULL THEN
               '未下线'
           ELSE
               '已下线'
       END State,
       tfr.MachineId,
       m.Name MachineName,
       tfr.Station StationCode,
       tfr.Station StationName
FROM dbo.[{unionTables.Item1[i]}] AS tc
    JOIN dbo.[{unionTables.Item2[i]}] AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.Machines AS m
        ON tfr.MachineId = m.Id
    LEFT JOIN dbo.DeviceGroups AS dg
        ON tc.DeviceGroupId = dg.Id
    LEFT JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
    LEFT JOIN dbo.ShiftSolutions AS ss
        ON msd.ShiftSolutionId = ss.Id
    LEFT JOIN dbo.ShiftSolutionItems AS ssi
        ON msd.ShiftSolutionItemId = ssi.Id
WHERE tc.Qualified != 1
      AND tfr.Tag = 2
{where}" + Environment.NewLine;
            }

            return executeSql;
        }

        public async Task<List<NGPartsExportDto>> ListNgPartsForExport(string partNo, DateTime? startTime, DateTime? endTime)
        {
            var where = string.Empty;
            var item1 = new List<string>();
            var item2 = new List<string>();

            if (!string.IsNullOrEmpty(partNo))
            {
                where += $" AND tc.PartNo LIKE '%{partNo.Trim()}%'";

                var tables =   this.GetUnionTables(DateTime.Now, DateTime.Now, partNo);

                item1.AddRange(tables.Item1);
                item2.AddRange(tables.Item2);
            }
            else
            {
                if (startTime.HasValue)
                {
                    where += $" AND '{startTime.Value}' <= tc.OnlineTime";

                    var tables =   this.GetUnionTables(startTime.Value.Date, null, partNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if ( endTime.HasValue)
                {
                    where += $" AND tc.OnlineTime <= '{endTime.Value}'";

                    var tables =   this.GetUnionTables(null, endTime.Value.Date, partNo);

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }

                if (!startTime.HasValue && !endTime.HasValue)
                {
                    var tables =  this.GetUnionTables(null, null, "-");//给随便一个工件值，走全部分表查询逻辑

                    item1.AddRange(tables.Item1);
                    item2.AddRange(tables.Item2);
                }
            }

            var unionTables = Tuple.Create(item1.Distinct().ToList(), item2.Distinct().ToList());

            var executeSql = $@"
SELECT tc.Id,
       tc.DeviceGroupId,
       dg.DisplayName DeviceGroupName,
       tc.PartNo,
       tc.OnlineTime,
       tc.OfflineTime,
       ssi.Id ShiftSolutionItemId,
       ss.Name + '-' + ssi.Name ShiftName,
       CASE
           WHEN tc.OfflineTime IS NULL THEN
               '未下线'
           ELSE
               '已下线'
       END State,
       tfr.MachineId,
       m.Name MachineName,
       tfr.Station StationCode,
       tfr.Station StationName
FROM dbo.TraceCatalogs AS tc
    JOIN dbo.TraceFlowRecords AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.Machines AS m
        ON tfr.MachineId = m.Id AND m.IsDeleted = 0
    LEFT JOIN dbo.DeviceGroups AS dg
        ON tc.DeviceGroupId = dg.Id
    LEFT JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
    LEFT JOIN dbo.ShiftSolutions AS ss
        ON msd.ShiftSolutionId = ss.Id
    LEFT JOIN dbo.ShiftSolutionItems AS ssi
        ON msd.ShiftSolutionItemId = ssi.Id
WHERE tc.Qualified = 0
      AND tfr.Tag = 2
{where}" + Environment.NewLine;

            for (int i = 0; i < unionTables.Item1.Count; i++)
            {
                //查询的数据涉及到分表
                executeSql += $@"
UNION ALL
SELECT tc.Id,
       tc.DeviceGroupId,
       dg.DisplayName DeviceGroupName,
       tc.PartNo,
       tc.OnlineTime,
       tc.OfflineTime,
       ssi.Id ShiftSolutionItemId,
       ss.Name + '-' + ssi.Name ShiftName,
       CASE
           WHEN tc.OfflineTime IS NULL THEN
               '未下线'
           ELSE
               '已下线'
       END State,
       tfr.MachineId,
       m.Name MachineName,
       tfr.Station StationCode,
       tfr.Station StationName
FROM dbo.[{unionTables.Item1[i]}] AS tc
    JOIN dbo.[{unionTables.Item2[i]}] AS tfr
        ON tc.PartNo = tfr.PartNo
    JOIN dbo.Machines AS m
        ON tfr.MachineId = m.Id
    LEFT JOIN dbo.DeviceGroups AS dg
        ON tc.DeviceGroupId = dg.Id
    LEFT JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
    LEFT JOIN dbo.ShiftSolutions AS ss
        ON msd.ShiftSolutionId = ss.Id
    LEFT JOIN dbo.ShiftSolutionItems AS ssi
        ON msd.ShiftSolutionItemId = ssi.Id
WHERE tc.Qualified = 0
      AND tfr.Tag = 2
{where}" + Environment.NewLine;
            }

            var query = new List<NGPartsExportDto>();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                query = (await conn.QueryAsync<NGPartsExportDto>(executeSql)).ToList();

                return query;
            }
        }
    }
}
