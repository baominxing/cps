using Abp.UI;
using Castle.Core.Logging;
using Dapper;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Common.Dtos;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Common
{
    public class CommonRepository : ICommonRepository
    {
        public ILogger Logger;

        public CommonRepository()
        {
            this.Logger = NullLogger.Instance;
        }

        private static Dictionary<string, Tuple<string, string>> CountryDic = new Dictionary<string, Tuple<string, string>>() {
            {"zh-CN",Tuple.Create("中文(简体)","famfamfam-flag-cn") },
            {"zh-TW",Tuple.Create("中文(繁体)","famfamfam-flag-tw") },
            {"en",Tuple.Create("英文","famfamfam-flag-gb") },
            {"de",Tuple.Create("德文","famfamfam-flag-de") },
            {"ja",Tuple.Create("日文","famfamfam-flag-jp") },
            {"fr",Tuple.Create("法文","famfamfam-flag-fr") },
            {"ru",Tuple.Create("俄文","famfamfam-flag-ru") },
            { "it",Tuple.Create("意大利文","famfamfam-flag-it")},
            { "ar",Tuple.Create("阿拉伯文","famfamfam-flag-ar")},
            { "nl",Tuple .Create("荷兰文","famfamfam-flag-nl")},
            { "th",Tuple.Create("泰文","famfamfam-flag-th")},
            { "ko",Tuple.Create("韩文","famfamfam-flag-kr")}
        };

        public List<AlarmInfo> RefillAlarmMessage()
        {
            const string sql = @"update Alarms  Set Alarms.Message = AlarmInfos.Message  from AlarmInfos  
                       where (LEN(Alarms.Message) = 0 OR Alarms.Message IS NULL)
                       and Alarms.MachineId = AlarmInfos.MachineId 
                       and LOWER(Alarms.Code) = LOWER(AlarmInfos.Code)";

            const string alarmInfosql = " SELECT ai.Code,ai.[Message],ai.Reason,ai.MachineId FROM AlarmInfos AS ai";


            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Execute(sql);
                var alarmInfos = conn.Query<AlarmInfo>(alarmInfosql).ToList();
 
                return alarmInfos;
            }
        }

        public string CallSpToSyncEndTime(bool alarmResult, bool stateResult, bool capacityResult)
        {
            string message = string.Empty;
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                const string updateNameSql = @" UPDATE States SET Name = A.DisplayName
                                             FROM ( SELECT DisplayName,Code FROM StateInfos ) AS A
                                             WHERE States.Name = '' AND States.Code = A.Code ";

                const string updateStateSql = @" UPDATE STATES
		                                         SET    STATES.EndTime = A.EndTime,
		                                               STATES.LastModificationTime =GETDATE(),
		                                               STATES.duration = DATEDIFF(S, STATES.StartTime, A.EndTime)
		                                         FROM   (
		         	                                        select s.Id,s.dmpId,sc.startTime as EndTime from states as s
		                                                    inner join states as sc on (s.dmpId = sc.PreviousLinkId and s.endTime is null)
		                                               ) AS A
		                                         WHERE  STATES.Id  = A.Id";

                const string updateCapacitySql = @"
		                                            UPDATE Capacities
		                                            SET    Capacities.EndTime = A.EndTime,
		                                                   Capacities.duration = DATEDIFF(S, Capacities.StartTime, A.EndTime)
		                                            FROM   (
		         	                                            select s.Id,s.dmpId,sc.startTime as EndTime from Capacities as s
		                                                        inner join Capacities as sc on (s.dmpId = sc.PreviousLinkId and s.endTime is null)
		                                                   ) AS A
		                                            WHERE  Capacities.Id  = A.Id";

                try
                {
                    if (stateResult)
                    {
                        conn.Execute(updateStateSql, commandTimeout: 600);
                    }
                    if (stateResult)
                    {
                        conn.Execute(updateNameSql, commandTimeout: 600);
                    }
                    if (capacityResult)
                    {
                        conn.Execute(updateCapacitySql, commandTimeout: 600);
                    }
                    this.Logger.Info($"[{DateTime.Now.ToLocalFormat()}]同步完成,更新字段成功");
                }
                catch (Exception e)
                {
                    this.Logger.Info($"[{DateTime.Now.ToLocalFormat()}]更新字段成功");
                    message = e.Message;
                }
            }

            return message;
        }

        public void UpdateColumnsAfterSyncState()
        {
            const string sql = @" UPDATE States SET Name = A.DisplayName
                                             FROM ( SELECT DisplayName,Code FROM StateInfos ) AS A
                                             WHERE States.Name = '' AND States.Code = A.Code ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Execute(sql);
            }
        }

        public async Task WriteLanguageToDatabase(MultiLanguageInputDto languageData)
        {
            try
            {
                using (var connection = new SqlConnection(AppSettings.Database.ConnectionString))
                {
                    connection.Open();
                    var selectSql = "select id from Languages where Name=@Name";
                    var langaugeId = connection.Query<int>(selectSql, new { Name = languageData.Culture }).FirstOrDefault();
                    if (langaugeId > 0)
                    {
                        return;
                    }
                    var sql = @"INSERT INTO Languages(
TenantId, 
Name,  
DisplayName, 
Icon, 
IsDeleted,
DeleterUserId,  
DeletionTime,
LastModificationTime,
LastModifierUserId,  
CreationTime,
CreatorUserId,
IsDisabled)
VALUES(
NULL,
@Name,
@DisplayName,
@Icon, 
0,
null,
null, 
null, 
null,
GETDATE(),
null,
0)";

                    await connection.ExecuteAsync(sql, new { Name = languageData.Culture, DisplayName = CountryDic[languageData.Culture].Item1, Icon = CountryDic[languageData.Culture].Item2 });
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"向数据库插入多语言数据失败,具体的错误信息：{ex.ToString()}");
            }

        }

        public List<string> GetUnionTables(DateTime startTime, DateTime endTime, string tableName)
        {
            var start = startTime.ToString("yyyy-MM-dd");
            var end = endTime.ToString("yyyy-MM-dd");

            var archiveTables = new List<string>();
            var sql = @$"SELECT s.ArchivedTable FROM dbo.ArchiveEntries AS s
WHERE s.TargetTable ='{tableName}'  AND  s.ArchiveValue BETWEEN '{start}'  AND '{end}'
GROUP BY s.ArchivedTable";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                archiveTables = conn.Query<string>(sql).ToList();
            }
            return archiveTables;
        }

        public List<DateTime> GetSummaryDateList(DateTime startTime, DateTime endTime)
        {
            var sql = @"SELECT  [Date] FROM  Calendars WHERE  DATE BETWEEN  @StartTime AND  @EndTime";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var returnValue = (conn.Query<DateTime>(sql, new { StartTime = startTime, EndTime = endTime })).ToList();

                return returnValue;
            }
        }

        public async Task<List<ShiftEffSummary>> GetShiftEffSummyList(string workShopCode)
        {
            var returnValue = new List<ShiftEffSummary>();

            var shiftDetialSql1 = @" SELECT T.MachineId,
                       T.MachinesShiftDetailId
                FROM   (
                           SELECT ROW_NUMBER() OVER(PARTITION BY s.MachineId ORDER BY s.Id DESC) AS 
                                  RowNum,
                                  s.MachineId,s.MachinesShiftDetailId
                           FROM   States AS s
                       ) T
                WHERE  T.RowNum = 1";
            var shiftDetialSql2 = @"SELECT P.Id AS MachinesShiftDetailId, P.MachineId
                         FROM   (
                                    SELECT ROW_NUMBER() OVER(PARTITION BY msd.MachineId ORDER BY msd.Id DESC) AS 
                                           RowNum,
                                           msd.*
                                    FROM   MachinesShiftDetails AS msd
                                           INNER JOIN (
                                                    SELECT T.MachineId,
                                                           T.MachinesShiftDetailId
                                                    FROM   (
                                                               SELECT ROW_NUMBER() OVER(PARTITION BY s.MachineId ORDER BY s.Id DESC) AS 
                                                                      RowNum,
                                                                      s.MachineId,
                                                                      s.MachinesShiftDetailId
                                                               FROM   States AS s
                                                           ) T
                                                    WHERE  T.RowNum = 1
                                                ) T
                                                ON  t.MachineId = msd.MachineId
                                                AND msd.Id < t.MachinesShiftDetailId
                                ) P
                         WHERE  P.RowNum = 1 ";

            // 当前班次，前一班次的数据获取方式
            var shiftDetialList = new[] { shiftDetialSql1, shiftDetialSql2 };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                for (var index = 0; index < shiftDetialList.Length; index++)
                {
                    var item = shiftDetialList[index];
                    var sql = this.GenerateEffSummarySql(item);

                    var result = (await conn.QueryAsync<EffSummaryObject>(sql, new { workShopCode })).ToArray();
                    var totalDuration = result.Sum(r => r.Duration);
                    if (totalDuration != 0)
                    {
                        result.ForEach(r => r.Rate = r.Duration / totalDuration);
                        var effSummaryObject = result.FirstOrDefault();
                        if (effSummaryObject != null)
                        {
                            returnValue.Add(
                                new ShiftEffSummary
                                {
                                    IsCurrentShift = index == 0,
                                    ShiftDesc = effSummaryObject.ShiftDesc,
                                    EffSummaryObjects = result.ToList()
                                });
                        }
                    }
                }
            }

            return returnValue.OrderBy(n => n.IsCurrentShift).ToList();
        }

        private string GenerateEffSummarySql(string item)
        {
            return $@"SELECT si.Hexcode,
                       si.DisplayName,
                       CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + ssi.Name AS shiftDesc,
                       SUM(s.Duration)                  AS Duration
                FROM states                           AS s
                       INNER JOIN (
                                {
                            item
                            }
                            )                           AS T
                            ON  (T.MachineId = s.MachineId AND T.MachinesShiftDetailId = s.MachinesShiftDetailId)
                       INNER JOIN MachinesShiftDetails  AS msd ON  msd.Id = s.MachinesShiftDetailId
                       INNER JOIN ShiftSolutionItems    AS ssi ON  ssi.Id = msd.ShiftSolutionItemId
                       INNER JOIN StateInfos            AS si ON  si.Code = s.Code
                        INNER JOIN MachineDeviceGroups AS mdg ON mdg.MachineId=s.MachineId
                WHERE mdg.DeviceGroupCode = @workShopCode
                GROUP BY si.Hexcode, si.DisplayName, ssi.StartTime,ssi.Name ,msd.ShiftDay ";
        }

        public async Task<List<UtilizationRate>> GetShiftUtilizationRateList(string workShopCode)
        {
            var utilizationRates = new List<UtilizationRate>();
            var sql = this.GenerateGetShiftUtilizationRateListSql();
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                try
                {
                    var result = (await conn.QueryAsync<ShiftUtilizationRate>(sql, new { workShopCode })).ToArray();

                    if (result.Any())
                    {
                        var groupByWorkShop = result.GroupBy(
                            g => g.MachineGroupName,
                            (key, g) => new { groupBy = key, list = g.ToList() });

                        groupByWorkShop.ForEach(
                            n => utilizationRates.Add(
                                new UtilizationRate { MachineGroupName = n.groupBy, UtilizationRateList = n.list }));
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }

                return utilizationRates;
            }
        }

        private string GenerateGetShiftUtilizationRateListSql()
        {
            return @"SELECT s.MachineId,
                        ssi.Id                           AS ShiftId,
                        ssi.Name                         AS ShiftName,
                        m.Name                           AS MachineName,
                        m.MachineGroupId,
                        dg.DisplayName                   AS MachineGroupName,
                        CONVERT(
                            DECIMAL(10, 2),
                            SUM(CASE WHEN si.IsPlaned = 1 THEN s.Duration ELSE 0 END) / ISNULL(
                                CASE 
                                    WHEN SUM(s.Duration) <= 2 THEN 1
                                    ELSE SUM(s.Duration)
                                END,
                                1
                            )
                            * 100) AS Rate
                FROM   States                           AS s
                        INNER JOIN StateInfos            AS si ON  si.Code = s.Code
                        INNER JOIN (
                                SELECT m.Id,
                                        m.Name,
                                        m.[Desc],
                                        mdg.DeviceGroupId AS MachineGroupId
                                FROM    MachineDeviceGroups AS mdg
                                        INNER JOIN Machines AS m ON  m.Id = mdg.MachineId
                                WHERE  mdg.DeviceGroupCode = @workShopCode
                            ) m
                            ON  m.id = s.MachineId
                        INNER JOIN (
                                SELECT T.MachineId,
                                        T.MachinesShiftDetailId
                                FROM   (
                                            SELECT ROW_NUMBER() OVER(PARTITION BY s.MachineId ORDER BY s.Id DESC) AS 
                                                    RowNum,
                                                    s.MachineId,
                                                    s.MachinesShiftDetailId
                                            FROM   States AS s
                                        ) T
                                WHERE  T.RowNum = 1
                            )                           AS T
                            ON  (
                                    T.MachineId = s.MachineId
                                    AND T.MachinesShiftDetailId = s.MachinesShiftDetailId
                                )
                        INNER JOIN MachinesShiftDetails  AS msd ON (msd.MachineId = T.MachineId AND msd.id = T.MachinesShiftDetailId)
                        INNER JOIN ShiftSolutionItems    AS ssi ON  ssi.Id = msd.ShiftSolutionItemId
                        INNER JOIN DeviceGroups AS dg ON dg.Id=m.MachineGroupId
                GROUP BY s.MachineId, ssi.Id,ssi.Name,m.Name, m.MachineGroupId, dg.DisplayName";
        }
    }
}
