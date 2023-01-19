using Abp.Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.QualifiedStatistics;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.QualifiedStatistics
{
    public class QualifiedStatisticsRepository: IQualifiedStatisticsRepository
    {
        private readonly ISettingManager _settingManager;

        public QualifiedStatisticsRepository(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }
        public async Task<List<QualificationData>> ListQualification(EnumStatisticalWays statisticalWay, string startTime, string endTime, List<int> deviceGroupId, List<int> shiftSolutionId, List<string> unionTables)
        {
            var groupBy = string.Empty;
            var select = string.Empty;
            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    select = "q.ShiftDay as StartTime,q.ShiftDayName ";
                    groupBy = "q.ShiftDay,q.ShiftDayName ";
                    break;
                case EnumStatisticalWays.ByWeek:
                    select = "q.ShiftWeekName as StartTime,q.ShiftWeekName ";
                    groupBy = "q.ShiftWeekName ";
                    break;
                case EnumStatisticalWays.ByMonth:
                    select = "q.ShiftMonthName as StartTime,q.ShiftMonthName ";
                    groupBy = "q.ShiftMonthName ";
                    break;
                case EnumStatisticalWays.ByYear:
                    select = "q.ShiftYear as StartTime,q.ShiftYearName ";
                    groupBy = "q.ShiftYear,q.ShiftYearName ";
                    break;
                case EnumStatisticalWays.ByShift:
                    select = " q.StartTime,q.MachineShiftDetailName ";
                    groupBy = " q.StartTime,q.MachineShiftDetailName ";
                    break;
                default:
                    throw new NotImplementedException();
            }

            var deviceGroupSql = deviceGroupId.Any() ? "AND dg.Id IN @DeviceGroupId " : " ";
            var shiftSolutionSql = shiftSolutionId.Any() ? " AND scv.ShiftSolutionId IN @shiftSolutionId " : " ";

            var unionQuery = $@"SELECT tc.PartNo,
					                   dg.DisplayName,
					                   tc.DeviceGroupId,
					                   tc.OfflineTime,
					                   tc.MachineShiftDetailId,
					                   tc.Qualified,
					                   scv.MachineShiftDetailName,
					                   scv.ShiftYear,
					                   scv.ShiftYearName,
					                   scv.ShiftMonth,
		                               scv.ShiftMonthName,
					                   scv.ShiftWeek,
		                               scv.ShiftWeekName,
		                               scv.ShiftDayName,scv.StartTime,
		                               scv.ShiftDay,scv.ShiftSolutionId
		                    FROM dbo.TraceCatalogs AS tc
				                    INNER JOIN dbo.DeviceGroups AS dg ON tc.DeviceGroupId = dg.Id
				                    INNER JOIN dbo.ShiftCalendarsView AS scv ON scv.MachineShiftDetailId = tc.MachineShiftDetailId
		                    WHERE scv.ShiftDay BETWEEN @startTime AND @endTime {deviceGroupSql}  {shiftSolutionSql} " + Environment.NewLine;

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL 
                                  SELECT tc.PartNo,
					                     dg.DisplayName,
					                     tc.DeviceGroupId,
					                     tc.OfflineTime,
					                     tc.MachineShiftDetailId,
					                     tc.Qualified,
					                     scv.MachineShiftDetailName,
					                     scv.ShiftYear,
					                     scv.ShiftYearName,
					                     scv.ShiftMonth,
		                                 scv.ShiftMonthName,
					                     scv.ShiftWeek,
		                                 scv.ShiftWeekName,
		                                 scv.ShiftDayName,scv.StartTime,
		                                 scv.ShiftDay,scv.ShiftSolutionId
		                    FROM dbo.[{item}] AS tc
				                    INNER JOIN dbo.DeviceGroups AS dg ON tc.DeviceGroupId = dg.Id
				                    INNER JOIN dbo.ShiftCalendarsView AS scv ON scv.MachineShiftDetailId = tc.MachineShiftDetailId
		                    WHERE scv.ShiftDay BETWEEN @startTime AND @endTime {deviceGroupSql}  {shiftSolutionSql} " + Environment.NewLine;
            }

            string executeSql =
                $@"WITH Quality
                    AS ( {unionQuery}  )
                    SELECT 
                     {select} AS SummaryDate,
                     q.DisplayName,
                     COUNT(q.PartNo) AS OnlineCount,
                     ISNULL(SUM (CASE WHEN q.OfflineTime IS NOT NULL AND q.Qualified = 1 THEN 1 ELSE 0 END),0) AS QualifiedOfflineCount,
                     ISNULL(SUM (CASE WHEN q.OfflineTime IS NULL AND PartNo IS NOT NULL THEN 1 ELSE 0 END),0) AS ProcessingCount,
                     ISNULL(SUM (CASE WHEN q.Qualified=0 AND q.OfflineTime IS NOT NULL THEN 1 ELSE 0 END),0) AS NGCount
                     FROM Quality AS q  
                     GROUP BY q.DisplayName, q.DeviceGroupId, {groupBy} 
                     order by StartTime      ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();

                var sqlParameters = new
                {
                    startTime,
                    endTime,
                    DeviceGroupId = deviceGroupId,
                    shiftSolutionId
                };

                var resultQualificationData = await conn.QueryAsync<QualificationData>(executeSql, sqlParameters);

                return resultQualificationData.ToList();
            }
        }
    }
}
