/****** Object:  UserDefinedFunction [dbo].[func_GetStateSummeryByDate]    Script Date: 2020/2/19 14:26:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:
-- 更新记录,返回数据包括两部分: 
-- 1、按照天返回统计，班次Id为空
-- 2、按照班次方案将当天的数据拆分成每个班次一条数据
-- 2017-03-28更新
-- 去掉 IsByShift,合并ShiftDay，SummaryDate为ShiftDay
-- 2018-01-30更新
-- 当EndTime为空时，默认取系统当前时间，避免状态长时间持续时duration计算不准确的问题
-- Creator:Howard.zhao
-- CreateTime:
-- ============================================= 
CREATE FUNCTION [dbo].[func_GetStateSummeryByDate]
(
	@StartTime         DATETIME,
	@EndTime           DATETIME,
	@MachineIdList     NVARCHAR(MAX)
)
RETURNS TABLE
AS
	RETURN 
	(
	    SELECT B.MachineId,
	           m.Name                     MachineName,
	           SUM(B.DebugDuration)       DebugDuration,
	           SUM(B.OfflineDuration)     OfflineDuration,
	           SUM(B.FreeDuration)        FreeDuration,
	           SUM(B.RunDuration)         RunDuration,
	           SUM(B.StopDuration)        StopDuration,
	           SUM(B.TotalDuration)       TotalDuration,
	           B.MachinesShiftDetailId,
	           B.ShiftDay
	    FROM   (
	               SELECT MachineId,
	                      CASE 
	                           WHEN Code = 'Debug' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
	                           ELSE 0
	                      END  AS DebugDuration,
	                      CASE 
	                           WHEN Code = 'Offline' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
	                           ELSE 0
	                      END  AS OfflineDuration,
	                      CASE 
	                           WHEN Code = 'Free' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
	                           ELSE 0
	                      END  AS FreeDuration,
	                      CASE 
	                           WHEN Code = 'Run' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
	                           ELSE 0
	                      END  AS RunDuration,
	                      CASE 
	                           WHEN Code = 'Stop' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
	                           ELSE 0
	                      END  AS StopDuration,
	                      ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0) AS 
	                      TotalDuration,
	                      A.MachinesShiftDetailId,
	                      A.ShiftDay
	               FROM   (
	                          SELECT s.MachineId,
	                                 CASE 
	                                      WHEN s.StartTime <= @StartTime THEN @StartTime
	                                      ELSE s.StartTime
	                                 END  AS StartTime,
	                                 CASE 
	                                      WHEN  ISNULL(s.EndTime,GETDATE()) > @EndTime THEN @EndTime
	                                      ELSE  ISNULL(s.EndTime,GETDATE())
	                                 END  AS EndTime,
	                                 Code,
	                                 msd.Id AS MachinesShiftDetailId,
	                                 msd.ShiftDay
	                          FROM   STATES s
	                                 INNER JOIN MachinesShiftDetails AS msd
	                                      ON  msd.Id = s.MachinesShiftDetailId
	                          WHERE  s.StartTime < @EndTime
	                                 AND (s.EndTime > @StartTime OR s.EndTime IS NULL)
	                                 AND (
	                                         @MachineIdList IS NULL
	                                         OR EXISTS(
	                                                SELECT 1
	                                                FROM   [func_SplitInts](@MachineIdList, ',')
	                                                WHERE  Item = s.MachineId
	                                            )
	                                     )
	                      ) A
	           ) B
	           INNER JOIN MACHINEs m ON  m.id = b.MachineId
	    GROUP BY
	           MachineId,
	           m.Name,
	           MachinesShiftDetailId,
	           ShiftDay
	)
GO


