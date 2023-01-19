using Abp.Application.Services.Dto;
using Abp.Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.Plan.Repository;
using Wimi.BtlCore.Plan.Repository.Dto;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.ProcessPlans
{
    public class PlanRepository : IPlanRepository
    {
        private readonly ISettingManager settingManager;
        private readonly string connectionString;

        public PlanRepository(ISettingManager settingManager)
        {
            this.settingManager = settingManager;

            this.connectionString = AppSettings.Database.ConnectionString;
        }

        public IEnumerable<PlanResponse> ListProcessPlans(ProcessPlanInput input)
        {
            var sql = $@" 
                        WITH Plans AS(
                        Select p.Id,p.PlanCode,p.PlanName,p.ProductName,p.PlanAmount,p.DeviceGroupId,
                               p.TargetType,p.TargetAmount,p.YieldSummaryType,p.YieldCounterMachineId,p.ProcessAmount,
	                           p.RealStartTime,
                               case when p.RealEndTime is null and p.RealStartTime is not null then GETDATE() else p.RealEndTime end RealEndTime
                        FROM ProcessPlans p
                        WHERE p.IsDeleted = 0
                        )

                        SELECT *
                        FROM Plans p
                        Where not (p.RealStartTime>@EndTime OR p.RealEndTime<@StartTime)";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<PlanResponse>(sql, new { input.StartTime, input.EndTime });
                return result;
            }
        }

        public IEnumerable<PlanResponse> ListStatisticalWayYieldByCapacity(ProcessPlanInput input)
        {
            var showColumn = string.Empty;
            var groupBy = string.Empty;
            var shiftJoin = string.Empty;

            switch (input.TargetType)
            {
                case EnumTargetDimension.ByDay:
                    showColumn = "CONVERT(nvarchar,c.ShiftDetail_ShiftDay,23)";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName, CONVERT(nvarchar,c.ShiftDetail_ShiftDay,23)";
                    break;
                case EnumTargetDimension.ByWeek:
                    showColumn = " ca.YYYYWeek+' 周'";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,ca.YYYYWeek+' 周' ";
                    break;
                case EnumTargetDimension.ByMonth:
                    showColumn = "ca.YYYYMM+' 月' ";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,ca.YYYYMM+' 月' ";
                    break;
                case EnumTargetDimension.ByYear:
                    showColumn = "CAST(ca.[Year] AS NVARCHAR)+' 年'";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,CAST(ca.[Year] AS NVARCHAR)+' 年' ";
                    break;
                case EnumTargetDimension.ByShift:
                    showColumn = "pt.ShiftTargetAmount TargetAmount,CONVERT(nvarchar,c.ShiftDetail_ShiftDay,23)+ ' ' +c.ShiftDetail_MachineShiftName";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,CONVERT(nvarchar,c.ShiftDetail_ShiftDay,23)+' ' +c.ShiftDetail_MachineShiftName,pt.ShiftTargetAmount";
                    shiftJoin = @"INNER JOIN MachinesShiftDetails msd ON MSD.Id = c.MachinesShiftDetailId
		                          LEFT JOIN PlanTargets pt On (pt.ProcessPlanId = p.Id AND pt.ShiftId = msd.ShiftSolutionItemId)";
                    break;
            }
            var unionQuery = $@"
SELECT Yield,ShiftDetail_ShiftDay,PlanId,Qualified,MachineId,MachinesShiftDetailId,ShiftDetail_MachineShiftName
FROM [Capacities] WHERE MachineId=@MachineId AND PlanId=@PlanId AND Qualified IS NULL AND ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime " + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                unionQuery += $@"
UNION ALL
SELECT Yield,ShiftDetail_ShiftDay,PlanId,Qualified,MachineId,MachinesShiftDetailId,ShiftDetail_MachineShiftName 
FROM [{item}] WHERE MachineId=@MachineId AND PlanId=@PlanId AND Qualified IS NULL AND ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime " + Environment.NewLine;
            }


            var sql = $@" 
                        SELECT  p.ProductName,
                                p.PlanName,
                                p.Id As PlanId,
                                {showColumn}       StatisticalWay,
                                SUM(c.Yield)        StatisticalWayAmount
                        FROM   ({unionQuery}) c
                                INNER JOIN Calendars ca  ON  ca.Date=CONVERT(nvarchar,c.ShiftDetail_ShiftDay,23)
                                INNER JOIN ProcessPlans  p  On (p.Id = c.PlanId)
                                {shiftJoin}
                        WHERE  c.ShiftDetail_ShiftDay Between @StartTime and @EndTime AND c.Qualified IS NULL AND c.MachineId=@MachineId  AND c.PlanId=@PlanId AND p.IsDeleted=0
                        {groupBy}";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<PlanResponse>(sql, new { input.StartTime, input.EndTime, input.PlanId, input.MachineId });

                return result;
            }
        }

        public IEnumerable<PlanResponse> ListStatisticalWayYieldByTrace(ProcessPlanInput input)
        {
            var showColumn = string.Empty;
            var groupBy = string.Empty;
            var shiftJoin = string.Empty;

            switch (input.TargetType)
            {
                case EnumTargetDimension.ByDay:
                    showColumn = "CONVERT(nvarchar,msd.ShiftDay,23)";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,msd.ShiftDay";
                    break;
                case EnumTargetDimension.ByWeek:
                    showColumn = " ca.YYYYWeek+' 周' ";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,ca.YYYYWeek+' 周' ";
                    break;
                case EnumTargetDimension.ByMonth:
                    showColumn = "ca.YYYYMM +' 月'";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,ca.YYYYMM+' 月' ";
                    break;
                case EnumTargetDimension.ByYear:
                    showColumn = "CAST(ca.[Year] AS NVARCHAR)+' 年'";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,CAST(ca.[Year] AS NVARCHAR)+' 年'";
                    break;
                case EnumTargetDimension.ByShift:
                    showColumn = "pt.ShiftTargetAmount TargetAmount,CONVERT(nvarchar,msd.ShiftDay,23)+' '+ssi.Name";
                    groupBy = "GROUP BY p.Id,p.ProductName,p.PlanName,CONVERT(nvarchar,msd.ShiftDay,23)+' '+ssi.Name,pt.ShiftTargetAmount";
                    shiftJoin = @"INNER JOIN ShiftSolutionItems ssi ON ssi.Id = msd.ShiftSolutionItemId
                                  LEFT JOIN PlanTargets pt On (pt.ProcessPlanId = p.Id AND pt.ShiftId = msd.ShiftSolutionItemId)";
                    break;
            }
            var unionQuery = $@"
SELECT tc.[Id],
       [PartNo],
       [OnlineTime],
       [OfflineTime],
       [DeviceGroupId],
       [Qualified],
       [IsReworkPart],
       [MachineShiftDetailId],
       [PlanId],
       [ArchivedTable]
FROM [TraceCatalogs] AS tc
    JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
WHERE msd.ShiftDay
BETWEEN @StartTime AND @EndTime" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                unionQuery += $@"
UNION ALL
SELECT tc.[Id],
       [PartNo],
       [OnlineTime],
       [OfflineTime],
       [DeviceGroupId],
       [Qualified],
       [IsReworkPart],
       [MachineShiftDetailId],
       [PlanId],
       [ArchivedTable]
FROM [{item}] AS tc
    JOIN dbo.MachinesShiftDetails AS msd
        ON tc.MachineShiftDetailId = msd.Id
WHERE msd.ShiftDay
BETWEEN @StartTime AND @EndTime" + Environment.NewLine;
            }

            var sql = $@" 
                        
                        SELECT p.ProductName,
                                p.PlanName,
                                p.Id As PlanId,
                                {showColumn}       StatisticalWay,
                                COUNT(Distinct PartNo)        StatisticalWayAmount

                        FROM ({unionQuery}) tc
                        INNER JOIN ProcessPlans p ON (tc.DeviceGroupId = p.DeviceGroupId AND tc.PlanId=p.Id)
                        INNER JOIN MachinesShiftDetails msd ON msd.id = tc.MachineShiftDetailId
                        INNER JOIN Calendars ca ON ca.Date=CONVERT(nvarchar,msd.ShiftDay,23)
                        {shiftJoin}
                        WHERE tc.Qualified = 1 AND p.Id = @PlanId AND p.IsDeleted=0
                        AND msd.ShiftDay  BETWEEN @StartTime AND @EndTime
                        {groupBy}";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<PlanResponse>(sql, new { input.StartTime, input.EndTime, input.PlanId });
                return result;
            }
        }

        public IEnumerable<SummaryDateDto> ListSummaryDate(ProcessPlanInput input)
        {
            string selectSql;
            var innerSql = string.Empty;
            var groupBySql = string.Empty;
            switch (input.TargetType)
            {
                case EnumTargetDimension.ByWeek:
                    selectSql = " DISTINCT c.YYYYISOWeek+' 周' ";
                    break;
                case EnumTargetDimension.ByMonth:
                    selectSql = " DISTINCT c.YYYYMM +' 月' ";
                    break;
                case EnumTargetDimension.ByYear:
                    selectSql = " DISTINCT CAST(c.[Year] AS NVARCHAR) +' 年'";
                    break;
                case EnumTargetDimension.ByShift:
                    selectSql = "ssi.Id ShiftId,CONVERT(nvarchar,c.Date,23)+' '+ssi.Name";
                    innerSql = @" INNER JOIN ShiftCalendars AS msd   ON msd.ShiftDay = c.[Date]
                                  INNER JOIN ShiftSolutionItems AS ssi ON msd.ShiftItemId = ssi.Id";
                    groupBySql = " GROUP BY c.[Date],ssi.Id ,ssi.Name";
                    break;
                default:
                    selectSql = " CONVERT(NVARCHAR, c.[Date], 23) ";
                    break;
            }

            var sql = $@"SELECT {selectSql} AS Value, CONVERT(NVARCHAR, c.[Date], 23) as Name
                        FROM   Calendars                        AS c
                        {innerSql}
                        WHERE  c.[Date] BETWEEN @StartTime AND @EndTime
                        {groupBySql} ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                return conn.Query<SummaryDateDto>(sql, new { input.StartTime, input.EndTime });
            }
        }

        public void HandlerLinePlan(TraceCatalog traceCatalog, bool qualified)
        {
            if (qualified)
            {
                try
                {
                    //0：新增 1:暂停 2：进行中 3 暂停
                    //获取当前正在执行的计划,有且只能由一个状态为“进行中”的计划
                    //选中自动开启下一个计划，必须选中自动结束当前计划
                    var selectSql = "select top 1* from ProcessPlans as pp WHERE  pp.IsDeleted=0 and pp.DeviceGroupId=@DeviceGroupId AND pp.[Status]=2";
                    var updateSql = "UPDATE ProcessPlans SET ";
                    using (var connection = new SqlConnection(this.connectionString))
                    {
                        //获取当前正在执行的计划
                        var plan = connection.QueryFirstOrDefault<ProcessPlan>(selectSql, new { DeviceGroupId = traceCatalog.DeviceGroupId });
                        if (plan != null && plan.YieldSummaryType == EnumYieldSummaryType.ByTraceOffline)
                        {

                            updateSql += "ProcessAmount =@ProcessAmount";
                            if (plan.ProcessAmount == 0)
                            {
                                updateSql += ",RealStartTime=getdate()";
                            }
                            var currentAmount = plan.ProcessAmount + 1;
                            if (currentAmount >= plan.PlanAmount)
                            {

                                //更新状态
                                if (plan.IsAutoFinishCurrentPlan)
                                {
                                    updateSql += ", [Status] =3,RealEndTime=getdate() ";
                                }
                                if (plan.IsAutoStartNextPlan)
                                {
                                    //currentAmount = plan.PlanAmount;
                                    var nextListSql = "SELECT * FROM ProcessPlans AS pp WHERE pp.[Status] IN (0,1,2) and pp.IsDeleted=0 and pp.DeviceGroupId=@DeviceGroupId  AND pp.Id!=@PlanId";
                                    var nextList = connection.Query<ProcessPlan>(nextListSql, new { PlanId = plan.Id, DeviceGroupId = traceCatalog.DeviceGroupId }).ToList();
                                    if (nextList.Count > 0)
                                    {
                                        //保证只能存在一个计划进行中
                                        if (!nextList.Any(n => n.Status == EnumPlanStatus.InProgress))
                                        {
                                            var next = nextList.Where(q => q.PlanStartTime != null).OrderBy(q => q.PlanStartTime.Value).FirstOrDefault();
                                            if (next == null)
                                            {
                                                next = nextList.FirstOrDefault();
                                            }
                                            var updateNextSql = "UPDATE ProcessPlans SET [Status]=2, RealStartTime=GetDate() where Id=@PlanId";
                                            connection.Execute(updateNextSql, new { PlanId = next.Id });
                                        }

                                    }
                                }
                            }
                            updateSql += " WHERE Id = @PlanId ";

                            connection.Execute(updateSql, new { ProcessAmount = currentAmount, PlanId = plan.Id });
                        }

                    }
                }

                catch (Exception)
                {
                    throw;
                    //Logger.Error("更新计划产量失败，方式：追溯下线");
                }
            }
        }

        public async Task<List<NameValueDto>> GetShiftSolutionName(List<int> machineIds, DateTime? planStartTime)
        {
            var returnValue = new List<NameValueDto>();
 
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var startTime = string.Empty;

                if (planStartTime.HasValue)
                {
                    startTime = " AND StartTime>=@StartTime";
                }

                var sql = $@" SELECT  DISTINCT  ShiftSolutionName AS NAME ,ShiftSolutionId AS VALUE  FROM ShiftCalendarsView
 WHERE  MachineId IN @MachineIds
{startTime}
";
                var res = await conn.QueryAsync<NameValueDto>(sql, new { MachineIds = machineIds, StartTime = planStartTime });

                if (res.Any())
                {
                    returnValue = res.ToList();
                }
            }

            return returnValue;
        }

        public void DeletePlanTarget(int planId)
        {
            using (var conn = new SqlConnection(this.connectionString))
            {
                var sql = @"DELETE FROM PlanTargets WHERE ProcessPlanId =@PlanId ";

                conn.Execute(sql, new { PlanId = planId });
            }
        }
    }
}