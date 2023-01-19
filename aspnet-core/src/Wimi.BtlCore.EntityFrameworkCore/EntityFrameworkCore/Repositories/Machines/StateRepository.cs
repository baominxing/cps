using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Castle.Components.DictionaryAdapter;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.ThirdpartyApis.Dto;
using WIMI.BTL.Machines.RepositoryDto.State;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    public class StateRepository : IStateRepository
    {
        private readonly string connectionString;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;

        public StateRepository(
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<Machine> machineRepository,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineRepository = machineRepository;
            this.archiveEntryRepository = archiveEntryRepository;
            this.connectionString = AppSettings.Database.ConnectionString;
        }

        public async Task<ShiftDefailInfo> GetShiftDefailInfo(int machineShiftDetailId)
        {
            const string sql = @"SELECT msd.ShiftDay,
                                       ssi.Name                       AS MachineShiftName,
                                       ss.Name                        AS SolutionName
                                FROM   MachinesShiftDetails           AS msd
                                       INNER JOIN ShiftSolutions      AS ss ON  ss.Id = msd.ShiftSolutionId
                                       INNER JOIN ShiftSolutionItems  AS ssi ON  ssi.Id = msd.ShiftSolutionItemId
                                WHERE  msd.Id = @machineShiftDetailId";
            using var conn = new SqlConnection(this.connectionString);
            return await conn.QueryFirstOrDefaultAsync<ShiftDefailInfo>(sql, new { machineShiftDetailId });
        }

        public string GetMaxSyncStateDateTime()
        {
            var sql = " SELECT MAX(s.MongoCreationTime) FROM States AS s  WHERE s.IsShiftSplit = 0 ";

            var unionTabls = ListUnionTable();

            foreach (var item in unionTabls)
            {
                sql += $"UNION ALL SELECT MAX(s.MongoCreationTime) FROM [{item}] AS s  WHERE s.IsShiftSplit = 0";
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
WHERE TargetTable = N'States'";

                using var conn = new SqlConnection(this.connectionString);

                var unionTables = conn.Query<string>(executeSql);

                return unionTables.ToList();
            }
        }

        public async Task<IEnumerable<ListMahcineStateMapDto>> ListMahcineStateMaps(DateTime startTime, DateTime endTime, List<long> machineIdList, List<string> unionTables)
        {
            var unionQuery = $@"
SELECT 
[Id],
[MachineId],
[Code],
[StartTime],
[EndTime],
[DateKey]
FROM dbo.States WHERE MachineId IN @MachineIdList AND [StartTime] >= @StartTime AND [StartTime] < @EndTime " + Environment.NewLine;

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"
UNION ALL 
SELECT 
[Id],
[MachineId],
[Code],
[StartTime],
[EndTime],
[DateKey]
FROM dbo.[{item}] WHERE MachineId IN @MachineIdList AND [StartTime] >= @StartTime AND [StartTime] < @EndTime" + Environment.NewLine;
            }

            var sql = $@"
WITH NSTATES
AS (SELECT s.[Id],
           [MachineId],
           s.[Code],
           [StartTime],
            SI.DisplayName  StateName,
           CASE
               WHEN [EndTime] IS NULL THEN
                   GETDATE()
               ELSE
                   [EndTime]
           END EndTime,
           [DateKey]
    FROM ({unionQuery}) AS S
  INNER JOIN StateInfos SI
            ON s.Code = SI.Code),
NREASONS
AS (SELECT RFR.Id,
		   RFR.[MachineId],
           RFR.[StateId],
           RFR.[StateCode],
           RFR.[StartTime],
           CASE
               WHEN RFR.[EndTime] IS NULL THEN
                   GETDATE()
               ELSE
                   RFR.[EndTime]
           END EndTime,
           RFR.[EndUserId],
           SI.DisplayName StateName
    FROM ReasonFeedbackRecords RFR
        INNER JOIN StateInfos SI
            ON RFR.StateId = SI.Id),
     TORI
AS (SELECT S.Id,
           S.MachineId,
           S.Code,
           S.StartTime,
           S.EndTime,
           S.DateKey,
           R.StateCode,
           R.StartTime RStartTime,
           R.EndTime REndTime,
            CASE
               WHEN R.StateName IS NULL THEN
                   s.StateName
               ELSE
                   R.StateName
           END StateName,
           ROW_NUMBER() OVER (PARTITION BY S.Id ORDER BY R.[StartTime]) AS rownum
    FROM NSTATES S
        LEFT JOIN NREASONS R
            ON (
                   S.MachineId = R.MachineId
                   AND (NOT (
                                (S.EndTime < R.StartTime)
                                OR (S.StartTime > R.EndTime)
                            )
                       )
               )),
     TTR
AS (SELECT T1.Id,
           T1.MachineId,
		   T1.Code,
           T1.DateKey,
           CASE
               WHEN T2.Id IS NULL THEN
                   T1.StartTime
               ELSE
                   T2.REndTime
           END AS StartTime,
           CASE
               WHEN T3.Id IS NULL THEN
                   T1.EndTime
               ELSE
                   T1.REndTime
           END EndTime,
           T1.StateCode,
           T1.RStartTime,
           T1.REndTime,
           T1.StateName 
    FROM TORI T1
        LEFT JOIN TORI T2
            ON (
                   T1.Id = T2.Id
                   AND T1.rownum - 1 = T2.rownum
               )
        LEFT JOIN TORI T3
            ON (
                   T1.Id = T3.Id
                   AND T1.rownum = T3.rownum - 1
               )),
     TOTAL
AS (SELECT TTR.*,
           CASE
               WHEN TTR.RStartTime IS NULL
                    OR TTR.RStartTime >= TTR.EndTime
                    OR TTR.REndTime <= TTR.StartTime THEN
                   'NONE'
               WHEN TTR.RStartTime <= TTR.StartTime
                    AND TTR.REndTime >= TTR.EndTime THEN
                   'OUT'
               WHEN TTR.RStartTime <= TTR.StartTime
                    AND TTR.REndTime > TTR.StartTime
                    AND TTR.REndTime < TTR.EndTime THEN
                   'LEFT'
               WHEN TTR.RStartTime > TTR.StartTime
                    AND TTR.RStartTime < TTR.EndTime
                    AND TTR.REndTime >= TTR.EndTime THEN
                   'RIGHT'
               WHEN TTR.RStartTime > TTR.StartTime
                    AND TTR.RStartTime < TTR.EndTime
                    AND TTR.REndTime > TTR.StartTime
                    AND TTR.REndTime < TTR.EndTime THEN
                   'IN'
               ELSE
                   'IMPOSSIBLE'
           END STATETYPE
    FROM TTR)

   SELECT * FROM TOTAL;
";
            using var conn = new SqlConnection(AppSettings.Database.ConnectionString);
            return await conn.QueryAsync<ListMahcineStateMapDto>(sql, new { StartTime = startTime, EndTime = endTime, MachineIdList = machineIdList });
        }

        public async Task<IEnumerable<ListMahcineStateMapDto>> ListMahcineStateMapsByShift(DateTime shiftDay, int machineId, List<string> unionTables)
        {
            var startTime = shiftDay;
            var endTime = shiftDay.AddDays(2);

            var unionQuery = $@"
SELECT
[Id],
[Code],
[DateKey],
[EndTime],
[MachineCode],
[MachineId],
[MachinesShiftDetailId],
[Memo],
[Name],
[OrderId],
[PartNo],
[ProductId],
[ProcessId],
[ProgramName],
[ShiftDetail_MachineShiftName],
[ShiftDetail_ShiftDay],
[ShiftDetail_SolutionName],
[ShiftDetail_StaffShiftName],
[StartTime],
[UserId],
[UserShiftDetailId]
FROM dbo.States WHERE ShiftDetail_ShiftDay = @StartTime AND MachineId = @MachineId " + Environment.NewLine;

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"
UNION ALL
SELECT
[Id],
[Code],
[DateKey],
[EndTime],
[MachineCode],
[MachineId],
[MachinesShiftDetailId],
[Memo],
[Name],
[OrderId],
[PartNo],
[ProductId],
[ProcessId],
[ProgramName],
[ShiftDetail_MachineShiftName],
[ShiftDetail_ShiftDay],
[ShiftDetail_SolutionName],
[ShiftDetail_StaffShiftName],
[StartTime],
[UserId],
[UserShiftDetailId]
FROM dbo.[{item}] WHERE ShiftDetail_ShiftDay = @StartTime AND MachineId = @MachineId " + Environment.NewLine;
            }

            var sql = $@"
WITH NSTATES
AS (SELECT [Id],
           [Code],
           [DateKey],
           CASE
               WHEN [EndTime] IS NULL THEN
                   GETDATE()
               ELSE
                   [EndTime]
           END EndTime,
           [MachineCode],
           [MachineId],
           [MachinesShiftDetailId],
           [Memo],
           [Name],
           [OrderId],
           [PartNo],
           [ProductId],
           [ProcessId],
           [ProgramName],
           [ShiftDetail_MachineShiftName],
           [ShiftDetail_ShiftDay],
           [ShiftDetail_SolutionName],
           [ShiftDetail_StaffShiftName],
           [StartTime],
           [UserId],
           [UserShiftDetailId]
    FROM ({unionQuery}) AS S
    WHERE MachineId = @MachineId
          AND [StartTime] >= @StartTime
          AND [StartTime] < @EndTime),
     NREASONS
AS (SELECT RFR.[MachineId],
           RFR.[StateId],
           RFR.[StateCode],
           RFR.[StartTime],
           CASE
               WHEN RFR.[EndTime] IS NULL THEN
                   GETDATE()
               ELSE
                   RFR.[EndTime]
           END EndTime,
           RFR.[EndUserId],
           SI.DisplayName RName
    FROM ReasonFeedbackRecords RFR
        INNER JOIN StateInfos SI
            ON RFR.StateId = SI.Id),
     TORI
AS (SELECT S.Id,
           S.Code,
           S.DateKey,
           S.EndTime,
           S.MachineCode,
           S.MachineId,
           S.MachinesShiftDetailId,
           S.Memo,
           S.Name,
           S.OrderId,
           S.PartNo,
           S.ProductId,
           S.ProcessId,
           S.ProgramName,
           S.ShiftDetail_MachineShiftName,
           S.ShiftDetail_ShiftDay,
           S.ShiftDetail_SolutionName,
           S.ShiftDetail_StaffShiftName,
           S.StartTime,
           S.UserId,
           S.UserShiftDetailId,
           R.StateCode,
           R.StartTime RStartTime,
           R.EndTime REndTime,
           R.RName,
           ROW_NUMBER() OVER (PARTITION BY S.Id ORDER BY R.[StartTime]) AS rownum
    FROM NSTATES S
        LEFT JOIN NREASONS R
            ON (
                   S.MachineId = R.MachineId
                   AND (NOT (
                                (S.EndTime < R.StartTime)
                                OR (S.StartTime > R.EndTime)
                            )
                       )
               )),
     TTR
AS (SELECT T1.Code,
           T1.DateKey,
           CASE
               WHEN T3.Id IS NULL THEN
                   T1.EndTime
               ELSE
                   T1.REndTime
           END EndTime,
           T1.MachineCode,
           T1.MachineId,
           T1.MachinesShiftDetailId,
           T1.Memo,
           T1.Name,
           T1.OrderId,
           T1.PartNo,
           T1.ProductId,
           T1.ProcessId,
           T1.ProgramName,
           T1.ShiftDetail_MachineShiftName,
           T1.ShiftDetail_ShiftDay,
           T1.ShiftDetail_SolutionName,
           T1.ShiftDetail_StaffShiftName,
           CASE
               WHEN T2.Id IS NULL THEN
                   T1.StartTime
               ELSE
                   T2.REndTime
           END AS StartTime,
           T1.UserId,
           T1.UserShiftDetailId,
           T1.StateCode,
           T1.RStartTime,
           T1.REndTime,
           T1.RName
    FROM TORI T1
        LEFT JOIN TORI T2
            ON (
                   T1.Id = T2.Id
                   AND T1.rownum - 1 = T2.rownum
               )
        LEFT JOIN TORI T3
            ON (
                   T1.Id = T3.Id
                   AND T1.rownum = T3.rownum - 1
               )),
     TOTAL
AS (SELECT TTR.*,
           CASE
               WHEN TTR.RStartTime IS NULL
                    OR TTR.RStartTime >= TTR.EndTime
                    OR TTR.REndTime <= TTR.StartTime THEN
                   'NONE'
               WHEN TTR.RStartTime <= TTR.StartTime
                    AND TTR.REndTime >= TTR.EndTime THEN
                   'OUT'
               WHEN TTR.RStartTime <= TTR.StartTime
                    AND TTR.REndTime > TTR.StartTime
                    AND TTR.REndTime < TTR.EndTime THEN
                   'LEFT'
               WHEN TTR.RStartTime > TTR.StartTime
                    AND TTR.RStartTime < TTR.EndTime
                    AND TTR.REndTime >= TTR.EndTime THEN
                   'RIGHT'
               WHEN TTR.RStartTime > TTR.StartTime
                    AND TTR.RStartTime < TTR.EndTime
                    AND TTR.REndTime > TTR.StartTime
                    AND TTR.REndTime < TTR.EndTime THEN
                   'IN'
               ELSE
                   'IMPOSSIBLE'
           END STATETYPE
    FROM TTR)
SELECT S.MachineId,S.DateKey,S.StartTime,S.EndTime,SI.DisplayName StateName,SI.Hexcode,S.Code StateCode,SSI.Name ShiftName,SSI.Id ShiftSolutionItemId,S.MachinesShiftDetailId
FROM
(
SELECT T.MachineId,
           T.StartTime,
           T.EndTime,
           T.Code,
           T.Name,
           DATEDIFF(SECOND, T.StartTime, T.EndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'NONE'
    UNION
    SELECT T.MachineId,
           T.StartTime,
           T.EndTime,
           T.StateCode Code,
           T.RName Name,
           DATEDIFF(SECOND, T.StartTime, T.EndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'OUT'
    UNION
    SELECT T.MachineId,
           T.StartTime,
           T.REndTime EndTime,
           T.StateCode Code,
           T.RName Name,
           DATEDIFF(SECOND, T.StartTime, T.REndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'LEFT'
    UNION
    SELECT T.MachineId,
           T.REndTime StartTime,
           T.EndTime,
           T.Code,
           T.Name,
           DATEDIFF(SECOND, T.REndTime, T.EndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'LEFT'
    UNION
    SELECT T.MachineId,
           T.StartTime,
           T.RStartTime EndTime,
           T.Code,
           T.Name,
           DATEDIFF(SECOND, T.StartTime, T.RStartTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'RIGHT'
    UNION
    SELECT T.MachineId,
           T.RStartTime StartTime,
           T.EndTime,
           T.StateCode Code,
           T.RName Name,
           DATEDIFF(SECOND, T.RStartTime, T.EndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'RIGHT'
    UNION
    SELECT T.MachineId,
           T.StartTime,
           T.RStartTime EndTime,
           T.Code,
           T.Name,
           DATEDIFF(SECOND, T.StartTime, T.RStartTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'IN'
    UNION
    SELECT T.MachineId,
           T.RStartTime StartTime,
           T.REndTime EndTime,
           T.StateCode Code,
           T.RName Name,
           DATEDIFF(SECOND, T.RStartTime, T.REndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'IN'
    UNION
    SELECT T.MachineId,
           T.REndTime StartTime,
           T.EndTime,
           T.Code,
           T.Name,
           DATEDIFF(SECOND, T.REndTime, T.EndTime) Duration,
           T.DateKey,
           T.MachineCode,
           T.MachinesShiftDetailId,
           T.Memo,
           T.OrderId,
           T.PartNo,
           T.ProductId,
           T.ProcessId,
           T.ProgramName,
           T.ShiftDetail_MachineShiftName,
           T.ShiftDetail_ShiftDay,
           T.ShiftDetail_SolutionName,
           T.ShiftDetail_StaffShiftName,
           T.UserId,
           T.UserShiftDetailId
    FROM TOTAL T
    WHERE T.STATETYPE = 'IN'
) AS S
INNER JOIN MachinesShiftDetails AS MSD ON S.MachinesShiftDetailId = MSD.Id
INNER JOIN ShiftSolutionItems AS SSI ON MSD.ShiftSolutionItemId = SSI.ID
INNER JOIN StateInfos AS SI ON S.Code = SI.Code
WHERE s.ShiftDetail_ShiftDay = @StartTime AND S.MachineId = @MachineId
                                    ";
            using var conn = new SqlConnection(AppSettings.Database.ConnectionString);
            var result = await conn.QueryAsync<ListMahcineStateMapDto>(sql, new { StartTime = startTime, EndTime = endTime, MachineId = machineId });

            return result;
        }

        public static async Task<IEnumerable<ListMahcineStateMapDto>> ListMahcineStateMapsByShift(DateTime shiftDay, int machineId)
        {
            var startTime = shiftDay;
            var endTime = shiftDay.AddDays(2);
            const string sql = @"SELECT S.MachineId,S.DateKey,S.StartTime,S.EndTime,SI.DisplayName StateName,SI.Hexcode,S.Code StateCode,SSI.Name ShiftName,SSI.Id ShiftSolutionItemId,S.MachinesShiftDetailId
                                 FROM func_ListMachineStates(@StartTime,@EndTime) AS S
                                 INNER JOIN MachinesShiftDetails AS MSD ON S.MachinesShiftDetailId = MSD.Id
								 INNER JOIN ShiftSolutionItems AS SSI ON MSD.ShiftSolutionItemId = SSI.ID
								 INNER JOIN StateInfos AS SI ON S.Code = SI.Code
								 WHERE s.ShiftDetail_ShiftDay = @StartTime
                                 AND S.MachineId = @MachineId
                                    ";
            using var conn = new SqlConnection(AppSettings.Database.ConnectionString);
            return await conn.QueryAsync<ListMahcineStateMapDto>(sql, new { StartTime = startTime, EndTime = endTime, MachineId = machineId });
        }

        public IEnumerable<dynamic> QueryCapacitiesBetweenStartTimeAndEndTime(DateTime startTime, DateTime endTime, List<string> unionTables)
        {
            var executeSql = $@"
SELECT [Id],
       [MachineId],
       [MachineCode],
       [Yield],
       [AccumulateCount],
       [OriginalCount],
       [Rate],
       [StartTime],
       [EndTime],
       [Duration],
       [ProgramName],
       [IsValid],
       [Memo],
       [UserId],
       [UserShiftDetailId],
       [MachinesShiftDetailId],
       [OrderId],
       [ProcessId],
       [PartNo],
       [DateKey],
       [ShiftDetail_SolutionName],
       [ShiftDetail_StaffShiftName],
       [ShiftDetail_MachineShiftName],
       [ShiftDetail_ShiftDay],
       [ProductId],
       [ProductName],
       [IsLineOutput],
       [Tag],
       [Qualified],
       [PlanId],
       [PlanName],
       [PlanAmount],
       [MongoCreationTime],
       [DmpId],
       [PreviousLinkId],
       [IsLineOutputOffline]
FROM [Capacities] WHERE ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime AND ShiftDetail_ShiftDay IS NOT NULL" + Environment.NewLine;

            foreach (var item in unionTables)
            {
                executeSql += $@"
UNION ALL
SELECT [Id],
       [MachineId],
       [MachineCode],
       [Yield],
       [AccumulateCount],
       [OriginalCount],
       [Rate],
       [StartTime],
       [EndTime],
       [Duration],
       [ProgramName],
       [IsValid],
       [Memo],
       [UserId],
       [UserShiftDetailId],
       [MachinesShiftDetailId],
       [OrderId],
       [ProcessId],
       [PartNo],
       [DateKey],
       [ShiftDetail_SolutionName],
       [ShiftDetail_StaffShiftName],
       [ShiftDetail_MachineShiftName],
       [ShiftDetail_ShiftDay],
       [ProductId],
       [ProductName],
       [IsLineOutput],
       [Tag],
       [Qualified],
       [PlanId],
       [PlanName],
       [PlanAmount],
       [MongoCreationTime],
       [DmpId],
       [PreviousLinkId],
       [IsLineOutputOffline]
FROM [{item}] WHERE ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime AND ShiftDetail_ShiftDay IS NOT NULL" + Environment.NewLine;
            }

            //优化人员产量报表加载速度
            var strSql = "select a.UserId,a.ShiftDetail_ShiftDay,ShiftCalendars.ShiftSolutionId,a.ShiftDetail_SolutionName,a.ShiftDetail_MachineShiftName,a.Yield," +
                " ShiftCalendars.Duration,ShiftCalendars.StartTime,ShiftCalendars.EndTime,ShiftSolutionItems.IsNextDay" +
                " From (" + executeSql + ") a" +
                " join ShiftCalendars on a.MachinesShiftDetailId=ShiftCalendars.MachineShiftDetailId " +
                " join ShiftSolutions on ShiftCalendars.ShiftSolutionId=ShiftSolutions.id" +
                " join ShiftSolutionItems on ShiftCalendars.ShiftItemId=ShiftSolutionItems.id" +
                " where a.UserId is not null ";

            var result = new List<dynamic>();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                result = conn.Query<dynamic>(strSql, new { StartTime = startTime, EndTime = endTime }).ToList();
            }

            return result;
        }

        public IEnumerable<dynamic> QueryStatesBetweenStartTimeAndEndTime(DateTime startTime, DateTime endTime, List<string> unionTables, bool byUser = false)
        {
            var groupByUser = byUser ? " ,[UserId] " : " ";

            var executeSql = $@"
 SELECT s.[MachineId],
           [ShiftDetail_ShiftDay],
           CASE
               WHEN s.EndTime IS NULL THEN
                   DATEDIFF(SECOND, s.StartTime, GETDATE())
               ELSE
                   s.[Duration]
           END AS [Duration],
           s.[Code],
           s.[Name],
           s.[MachinesShiftDetailId],
           [ShiftDetail_SolutionName],
           [ShiftDetail_MachineShiftName],
           scv.StartTime
           {groupByUser}
    FROM [States] s
    INNER JOIN ShiftCalendarsView scv ON scv.MachineShiftDetailId = s.MachinesShiftDetailId
    WHERE ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime AND ShiftDetail_ShiftDay IS NOT NULL " + Environment.NewLine;

            foreach (var item in unionTables)
            {
                executeSql += $@"
UNION ALL
 SELECT s.[MachineId],
           [ShiftDetail_ShiftDay],
           CASE
               WHEN s.EndTime IS NULL THEN
                    DATEDIFF(SECOND, s.StartTime, GETDATE())
               ELSE
                   s.[Duration]
           END AS [Duration],
           s.[Code],
           s.[Name],
           s.[MachinesShiftDetailId],
           [ShiftDetail_SolutionName],
           [ShiftDetail_MachineShiftName],
           scv.StartTime
           {groupByUser}
FROM [{item}] AS s
INNER JOIN ShiftCalendarsView scv ON scv.MachineShiftDetailId = s.MachinesShiftDetailId
WHERE ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime AND ShiftDetail_ShiftDay IS NOT NULL" + Environment.NewLine;
            }

            var result = new List<dynamic>();

            var sql = $@"SELECT m.Name AS MachineName,
       dg.DisplayName AS MachineGroupName,
       T.MachineId,
       T.ShiftDetail_ShiftDay,
       T.MachinesShiftDetailId,
       T.ShiftDetail_SolutionName,
       T.ShiftDetail_MachineShiftName,
       SUM(T.Duration) AS Duration,
       T.Code,
       T.Name,
	   T.StartTime {groupByUser}
FROM
({executeSql}) AS T
    INNER JOIN dbo.Machines AS m ON m.Id = T.MachineId
    INNER JOIN dbo.MachineDeviceGroups mdg ON mdg.MachineId = m.Id
    INNER JOIN dbo.DeviceGroups dg ON dg.Id = mdg.DeviceGroupId
GROUP BY m.Name, dg.DisplayName,T.MachineId, T.ShiftDetail_ShiftDay,T.Code,T.Name,T.MachinesShiftDetailId, T.ShiftDetail_SolutionName,T.ShiftDetail_MachineShiftName,T.StartTime {groupByUser}";
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                result = conn.Query<dynamic>(sql, new { StartTime = startTime, EndTime = endTime }).ToList();
            }

            return result;
        }

        public async Task<List<MachineStateRateDto>> GetMachineStateRateData(GetMachineStateRateInputDto input)
        {
            string summaryDateColumn = string.Empty;
            string shiftJoin = string.Empty;
            string selectedDeviceGroup = string.Empty;
            string shiftCalendarsViewJoin = string.Empty;
            string startTimeSeq = string.Empty;
            string groupByStartTime = string.Empty;
            string orderByStartTime = string.Empty;

            string selectSortSeq = string.Empty;
            string groupBySortSeq = string.Empty;
            string orderBySortSeq = string.Empty;

            var MachineIdList = input.MachineIdList;
            switch (input.StatisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    summaryDateColumn = "CONVERT(NVARCHAR(10), s.ShiftDay, 120)";
                    orderByStartTime = "CONVERT(NVARCHAR(10), s.ShiftDay, 120)";
                    break;
                case EnumStatisticalWays.ByWeek:
                    summaryDateColumn = "c.ShiftWeekName";
                    shiftCalendarsViewJoin = " JOIN ShiftCalendarsView             AS c ON   c.MachineShiftDetailId = s.MachinesShiftDetailId ";
                    orderByStartTime = "c.ShiftWeekName";
                    break;
                case EnumStatisticalWays.ByMonth:
                    summaryDateColumn = "c.ShiftMonthName";
                    shiftCalendarsViewJoin = " JOIN ShiftCalendarsView             AS c ON   c.MachineShiftDetailId = s.MachinesShiftDetailId ";
                    orderByStartTime = "c.ShiftMonthName";
                    break;
                case EnumStatisticalWays.ByYear:
                    summaryDateColumn = "c.ShiftYearName";
                    shiftCalendarsViewJoin = " JOIN ShiftCalendarsView             AS c ON   c.MachineShiftDetailId = s.MachinesShiftDetailId ";
                    orderByStartTime = "c.ShiftYearName";
                    break;
                case EnumStatisticalWays.ByShift:
                    shiftJoin = @"JOIN MachinesShiftDetails  AS msd ON  msd.Id = s.MachinesShiftDetailId 
                                  JOIN ShiftSolutionItems    AS ssi ON  ssi.Id = msd.ShiftSolutionItemId ";

                    summaryDateColumn = "CONVERT(NVARCHAR(10), s.ShiftDay, 120) + ' ' + ssi.Name";
                    shiftCalendarsViewJoin = " JOIN ShiftCalendarsView             AS c ON   c.MachineShiftDetailId = s.MachinesShiftDetailId  AND  c.ShiftSolutionName IN @MachineShiftSolutionNameList ";
                    groupByStartTime = " c.StartTime,";
                    orderByStartTime = "c.StartTime";
                    break;
            }

            var summaryIdColumn = string.Empty;
            var summaryNameColumn = string.Empty;
            switch (input.QueryType)
            {
                case "0":
                    summaryIdColumn = "m.Id";
                    summaryNameColumn = "m.NAME";
                    selectSortSeq = "m.SortSeq   AS SortSeq,";
                    groupBySortSeq = " m.SortSeq,";
                    orderBySortSeq = ",m.SortSeq";

                    break;
                default:
                    summaryIdColumn = "mdg.DeviceGroupId";
                    summaryNameColumn = "dg.DisplayName";
                    selectedDeviceGroup = "WHERE mdg.DeviceGroupId IN @DeviceGroupIdList";
                    MachineIdList = this.GetMachineIdListViaMachineGroupIdList(input.MachineIdList).ToList();
                    break;
            }
            //根据查询时间范围，获取需要union的分表
            input.UnionTables = this.GetUnionTables(input);

            var unionQuery = $@" SELECT MachineId,
                                   CASE
                                       WHEN EndTime IS NULL THEN
                                           DATEDIFF(SECOND,StartTime, GETDATE())
                                       ELSE
                                           [Duration]
                                   END AS [Duration],
                                   Code,
                                   MachinesShiftDetailId,
                                   CONVERT(NVARCHAR(10), ShiftDetail_ShiftDay, 120) AS ShiftDay
                            FROM dbo.States
                           WHERE MachineId IN @MachineIdList AND ShiftDetail_ShiftDay  BETWEEN @StartTime AND  @EndTime " + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL 
                  SELECT MachineId,
                   CASE
                       WHEN EndTime IS NULL THEN
                            DATEDIFF(SECOND,StartTime, GETDATE())
                       ELSE
                           [Duration]
                   END AS [Duration],
                   Code,
                   MachinesShiftDetailId,
                   CONVERT(NVARCHAR(10), ShiftDetail_ShiftDay, 120) AS ShiftDay
             FROM dbo.[{item}] WHERE MachineId IN @MachineIdList AND ShiftDetail_ShiftDay  BETWEEN @StartTime AND  @EndTime " + Environment.NewLine;
            }
            var machineIdParams = MachineIdList.Any() ? "AND s.MachineId IN @MachineIdList" : " ";
            string executeSql = $@"
SELECT 
        {selectSortSeq}
        {startTimeSeq} 
        {summaryIdColumn}             SummaryId,
       {summaryNameColumn}           SummaryName,
       {summaryDateColumn}           SummaryDate,
       s.Code,  SUM( s.Duration) AS Duration
FROM   ({unionQuery})  s
          
       {shiftCalendarsViewJoin}
       JOIN StateInfos AS si  ON si.Code = s.Code
       JOIN [Machines]            AS m ON  m.Id = s.MachineId
       JOIN MachineDeviceGroups   AS mdg ON  mdg.MachineId = m.Id
       JOIN DeviceGroups          AS dg ON  dg.Id = mdg.DeviceGroupId
       {shiftJoin}
{selectedDeviceGroup}
GROUP BY 
        {groupBySortSeq}
        {groupByStartTime}
       {summaryIdColumn},
       {summaryNameColumn},
       {summaryDateColumn},
  s.Code
ORDER BY
{orderByStartTime}
{orderBySortSeq}
";
            var originalMachineStateRateList = new List<MachineStateRateDto>();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();

                executeSql = executeSql.Replace("@StartTime", $@"'{input.StartTime}'")
                    .Replace("@EndTime", $@"'{input.EndTime}'")
                    .Replace("@MachineIdList", $@"({MachineIdList.JoinAsString(",")})")
                    .Replace("@DeviceGroupIdList", $@"({input.MachineIdList.JoinAsString(",")})")
                    .Replace("@MachineShiftSolutionNameList", $@"('{input.MachineShiftSolutionNameList.JoinAsString(",")}')");
                originalMachineStateRateList = (await conn.QueryAsync<MachineStateRateDto>(executeSql)).ToList();
            }

            var query = originalMachineStateRateList.GroupBy(t => new
            {
                t.SummaryDate,
                t.SummaryId,
                t.SummaryName,
                t.SortSeq
            }, (key, g) => new
            {
                key.SummaryDate,
                key.SummaryId,
                key.SummaryName,
                key.SortSeq,
                RunDuration = g.Where(s => s.Code == "Run").Sum(s => s.Duration),
                DebugDuration = g.Where(s => s.Code == "Debug").Sum(s => s.Duration),
                StopDuration = g.Where(s => s.Code == "Stop").Sum(s => s.Duration),
                FreeDuration = g.Where(s => s.Code == "Free").Sum(s => s.Duration),
                OfflineDuration = g.Where(s => s.Code == "Offline").Sum(s => s.Duration),
                TotalDuration = g.Sum(s => s.Duration),
            }).ToList();

            // 处理数据
            return query.Select(t => new MachineStateRateDto
            {
                SummaryName = t.SummaryName,
                SummaryDate = t.SummaryDate,
                SortSeq = t.SortSeq,
                SummaryId = t.SummaryId,
                RunDuration = t.RunDuration,
                RunDurationRate = t.RunDuration == 0 ? 0 : Math.Round(t.RunDuration / t.TotalDuration, 4),
                StopDuration = t.StopDuration,
                StopDurationRate = t.StopDuration == 0 ? 0 : Math.Round(t.StopDuration / t.TotalDuration, 4),
                OfflineDuration = t.OfflineDuration,
                OfflineDurationRate = t.OfflineDuration == 0 ? 0 : Math.Round(t.OfflineDuration / t.TotalDuration, 4),
                FreeDuration = t.FreeDuration,
                FreeDurationRate = t.FreeDuration == 0 ? 0 : Math.Round(t.FreeDuration / t.TotalDuration, 4),
                DebugDuration = t.DebugDuration,
                DebugDurationRate = t.DebugDuration == 0 ? 0 : Math.Round(t.DebugDuration / t.TotalDuration, 4),
            }).ToList();
        }

        private IEnumerable<int> GetMachineIdListViaMachineGroupIdList(List<int> idList)
        {
            var machineIdList = (from uou in this.machineDeviceGroupRepository.GetAll()
                                 join m in this.machineRepository.GetAll() on uou.MachineId equals m.Id
                                 where idList.Contains(uou.DeviceGroupId)
                                 select m.Id).ToList();

            return machineIdList;
        }

        private List<string> GetUnionTables(GetMachineStateRateInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => Convert.ToDateTime(input.StartTime) <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= Convert.ToDateTime(input.EndTime))
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }

        public async Task<IEnumerable<MachineStateRateOutputDto>> GetMachineStateRate(int? machineId, IEnumerable<ShiftCalendarDto> correctedQueryDateList)
        {
            var returnValue = new List<MachineStateRateOutputDto>();

            var sql = @"
SELECT A.MachineId,
       A.Code,
       si.DisplayName,
       si.Hexcode,
       CAST(
           (
               CAST(SUM(DATEDIFF(second, A.StartTime, A.EndTime)) AS FLOAT) / A.TotalDuration
           ) * 100 AS DECIMAL(10, 2)
       )                AS Rate
FROM   (
           SELECT s.MachineId,
                  s.Code,
                  CASE 
                       WHEN s.StartTime <= @StartTime THEN @StartTime
                       ELSE s.StartTime
                  END AS StartTime,
                  CASE 
                       WHEN s.EndTime > @EndTime
           OR s.EndTime IS NULL THEN @EndTime
              ELSE s.EndTime
              END AS EndTime,
           T.TotalDuration
           FROM States AS s
           INNER JOIN (
               SELECT A.MachineId,
                      SUM(DATEDIFF(SECOND, StartTime, EndTime)) AS TotalDuration
               FROM   (
                          SELECT s.MachineId,
                                 CASE 
                                      WHEN s.StartTime <= @StartTime THEN @StartTime
                                      ELSE s.StartTime
                                 END AS StartTime,
                                 CASE 
                                      WHEN s.EndTime > @EndTime
                          OR s.EndTime IS NULL THEN @EndTime
                             ELSE s.EndTime
                             END AS EndTime,
                          Code
                          FROM STATES s
                          WHERE s.StartTime < @EndTime
                          AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
                          AND s.MachineId IN @MachineIdList
                      ) A
               GROUP BY
                      A.MachineId
           ) AS T
           ON T.MachineId = s.MachineId
           WHERE s.StartTime < @EndTime
           AND (s.EndTime > @StartTime OR s.EndTime IS NULL) AND s.MachineId IN @MachineIdList
       )                AS A
       JOIN StateInfos  AS si
            ON  si.Code = A.Code
GROUP BY
       A.MachineId,
       A.Code,
       si.DisplayName,
       si.Hexcode,
       A.TotalDuration
";

            foreach (var item in correctedQueryDateList)
            {
                using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
                {
                    var param = new
                    {
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        MachineIdList = new EditableList<int> { machineId.Value }
                    };
                    returnValue.AddRange((await conn.QueryAsync<MachineStateRateOutputDto>(sql, param)).ToList());
                }
            }

            return returnValue;
        }
 
        public async Task<IEnumerable<GetOriginalState>> GetOriginalState(List<int>machineIdList, DateTime? startTime, DateTime? endTime)
        {
            IEnumerable<GetOriginalState> returnValue = new List<GetOriginalState>();
  
            var sql = @"
SELECT s.MachineId            AS Id,
       m.Name,
       si.Code         AS code,
       CASE 
            WHEN s.StartTime <= @StartTime THEN @StartTime
            ELSE s.StartTime
       END                    AS startDatetime,
       CASE 
            WHEN s.EndTime > @EndTime
                 OR s.EndTime IS NULL THEN @EndTime
            ELSE s.EndTime
       END                    AS endDatetime,
       s.Code,
       si.[Type],
       si.Hexcode             AS color
FROM   States                 AS s
       INNER JOIN StateInfos  AS si
            ON  si.Code = s.Code
       INNER JOIN Machines    AS m
            ON  m.id = s.MachineId
WHERE  s.StartTime < @EndTime
       AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
       AND s.MachineId IN @MachineIdList
ORDER BY m.SortSeq
";

            var reasonSql = @"
SELECT s.MachineId            AS Id,
       m.Name,
       si.DisplayName         AS displayName,
       CASE 
            WHEN s.StartTime <= @StartTime THEN @StartTime
            ELSE s.StartTime
       END                    AS startDatetime,
       CASE 
            WHEN s.EndTime > @EndTime
                 OR s.EndTime IS NULL THEN @EndTime
            ELSE s.EndTime
       END                    AS endDatetime,
       si.Code,
       si.[Type],
       si.Hexcode             AS color
FROM   ReasonFeedbackRecords  AS s
       INNER JOIN StateInfos  AS si
            ON  si.Id = s.StateId
       INNER JOIN Machines    AS m
            ON  m.id = s.MachineId
WHERE  s.IsDeleted = 0
       AND s.StartTime < @EndTime
       AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
       AND s.MachineId IN @MachineIdList
ORDER BY m.SortSeq
";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    StartTime =startTime,
                    EndTime = endTime,
                    MachineIdList =machineIdList
                };

                var result = await conn.QueryAsync<GetOriginalState>(sql, param);
                var reasonResult = await conn.QueryAsync<GetOriginalState>(reasonSql, param);


                if (!result.Any())
                {
                    return new List<GetOriginalState>();
                }
                else
                {
                    returnValue = result;
                }
 
            }

            return returnValue;
        }

        public async Task<IEnumerable<GetOriginalState>> GetOriginalReasonState(List<int> machineIdList, DateTime? startTime, DateTime? endTime)
        {
            IEnumerable<GetOriginalState> returnValue = new List<GetOriginalState>();
  
            var reasonSql = @"
SELECT s.MachineId            AS Id,
       m.Name,
       si.DisplayName         AS displayName,
       CASE 
            WHEN s.StartTime <= @StartTime THEN @StartTime
            ELSE s.StartTime
       END                    AS startDatetime,
       CASE 
            WHEN s.EndTime > @EndTime
                 OR s.EndTime IS NULL THEN @EndTime
            ELSE s.EndTime
       END                    AS endDatetime,
       si.Code,
       si.[Type],
       si.Hexcode             AS color
FROM   ReasonFeedbackRecords  AS s
       INNER JOIN StateInfos  AS si
            ON  si.Id = s.StateId
       INNER JOIN Machines    AS m
            ON  m.id = s.MachineId
WHERE  s.IsDeleted = 0
       AND s.StartTime < @EndTime
       AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
       AND s.MachineId IN @MachineIdList
ORDER BY m.SortSeq
";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    MachineIdList = machineIdList
                };

 
                var reasonResult = await conn.QueryAsync<GetOriginalState>(reasonSql, param);


                if (!reasonResult.Any())
                {
                    return new List<GetOriginalState>();
                }
                else
                {
                    returnValue = reasonResult;
                }

            }

            return returnValue;
        }

        public async Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate(List<int> machineIdList, DateTime startTime, DateTime endTime,int? machineId)
        {
            var returnValue = new List<UtilizationRateOutputDto>();

            var sql = @"
SELECT C.MachineId,
       m.Name            MachineName,
       CONVERT(
           DECIMAL(18, 2),
           CAST(
               SUM(CASE WHEN C.Code = 'Run' THEN C.Duration ELSE 0 END) 
               AS FLOAT
           ) / NULLIF(SUM(C.Duration),0) * 100
       )
       UtilizationRate
FROM   (
           SELECT A.MachineId,
                  DATEDIFF(s, A.StartTime, A.EndTime) Duration,
                  A.Code
           FROM   (
                      SELECT s.MachineId,
                             CASE 
                                  WHEN s.StartTime <= @StartTime THEN @StartTime
                                  ELSE s.StartTime
                             END AS StartTime,
                             CASE 
                                  WHEN s.EndTime > @EndTime
                      OR s.EndTime IS NULL THEN @EndTime
                         ELSE s.EndTime
                         END AS EndTime,
                      s.Code
                      FROM STATES s
                      WHERE s.StartTime < @EndTime
                      AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
                      AND s.MachineId IN @MachineIdList
                  )
                  A
       ) C
       JOIN Machines  AS m
            ON  m.id = c.MachineId
GROUP BY
       C.MachineId,
       m.Name
";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var param = new
                {
                    MachineIdList= machineIdList,
                    StartTime = startTime,
                    EndTime = endTime
                };

                var result = (await conn.QueryAsync<UtilizationRateOutputDto>(sql, param)).ToArray();
                if (result.Any())
                {
                    returnValue.AddRange(result);
                }
                else
                {
                    if (machineId != null)
                        returnValue.Add(new UtilizationRateOutputDto { MachineId = machineId.Value });
                }
            }

            return returnValue;
        }

        public async Task<IEnumerable<MachineYieldAnalysisOutputDto>> GetMachineData(List<int> machineIdList)
        {

            var machineSql = @"SELECT m.Name                   AS MachineName,
                                      m.Id                     AS MachineId,
                                      mdg.DeviceGroupId        AS MachineGroupId,
                                      dg.DisplayName           AS MachineGroupName
                               FROM   Machines                 AS m
                                      INNER JOIN MachineDeviceGroups AS mdg ON  mdg.MachineId = m.Id
                                      INNER JOIN DeviceGroups  AS dg ON  dg.Id = mdg.DeviceGroupId
                                      WHERE dg.IsDeleted = 0 and m.Id IN @MachineIdList
                               ORDER BY m.SortSeq,m.Code";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var machines = await conn.QueryAsync<MachineYieldAnalysisOutputDto>(machineSql, new { MachineIdList= machineIdList });
 
                return machines;
            }
        }

        public async Task<List<MachineYieldAnalysisOutputDto>> GetMachineYieldData(List<int> machineIdList, DateTime startTime, DateTime endTime)
        {
            var capacityFilter = Convert.ToBoolean(AppSettings.TraceabilityConfig.OfflineYield) ? "" : " AND c.IsLineOutput = 0 ";
            var sql = $@"
SELECT m.Name                          AS MachineName,
       m.Id                            AS MachineId,
       mdg.DeviceGroupId               AS MachineGroupId,
       dg.DisplayName                  AS MachineGroupName,
       ISNULL(A.Yield, 0)                 Yield,
       CONVERT(
           DECIMAL(18, 2),
           ISNULL(B.UtilizationRate, 0)
       )       UtilizationRate
FROM   Machines                        AS m
       INNER JOIN MachineDeviceGroups  AS mdg
            ON  mdg.MachineId = m.Id
       INNER JOIN DeviceGroups         AS dg
            ON  dg.Id = mdg.DeviceGroupId
       LEFT JOIN (
                SELECT c.MachineId,
                       SUM(c.Yield)     Yield
                FROM   Capacities    AS c
                WHERE  c.StartTime BETWEEN @StartTime AND @EndTime {capacityFilter}
                       AND c.MachineId IN @MachineIdList
                GROUP BY
                       c.MachineId
            )                          AS A
            ON  A.MachineId = m.Id
       LEFT JOIN (
                SELECT C.MachineId,
                       CAST(
                           SUM(CASE WHEN C.Code = 'Run' THEN C.Duration ELSE 0 END) 
                           AS FLOAT
                       ) / NULLIF(SUM(C.Duration),0) * 100 UtilizationRate
                FROM   (
                           SELECT A.MachineId,
                                  DATEDIFF(s, A.StartTime, A.EndTime) Duration,
                                  A.Code
                           FROM   (
                                      SELECT s.MachineId,
                                             CASE 
                                                  WHEN s.StartTime <= @StartTime THEN 
                                                       @StartTime
                                                  ELSE s.StartTime
                                             END AS StartTime,
                                             CASE 
                                                  WHEN s.EndTime > @EndTime OR s.EndTime IS NULL THEN @EndTime
                                            ELSE s.EndTime
                                            END AS EndTime,
                                            s.Code
                                      FROM STATES s
                                      WHERE s.StartTime < @EndTime
                                      AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
                                      AND s.MachineId IN @MachineIdList
                                  )
                                  A
                       ) C
                GROUP BY
                       C.MachineId
            )                          AS B
            ON  B.MachineId = m.Id
WHERE  dg.IsDeleted = 0
       AND m.Id IN @MachineIdList
ORDER BY m.SortSeq
";

            var param = new
            {
                MachineIdList = machineIdList,
                StartTime = startTime,
                EndTime = endTime
            };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                 var result = (await conn.QueryAsync<MachineYieldAnalysisOutputDto>(sql, param)).ToList();

                return result;
            }
        }
    }
}