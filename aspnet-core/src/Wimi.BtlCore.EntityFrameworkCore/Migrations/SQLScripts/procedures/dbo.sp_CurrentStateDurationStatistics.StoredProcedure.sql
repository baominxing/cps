/****** Object:  StoredProcedure [dbo].[sp_CurrentStateDurationStatistics]    Script Date: 2020/2/19 14:48:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	按条件汇总统计数据至DailyStatesSummaries表
-- 2017-03-28 更新
-- DailyStatesSummaries 去掉了 IsByShift字段，合并SummaryDate,ShiftDay 为ShiftDay
-- Creator: fred.bao
-- CreateTime: 

-- * 默认最大汇总间隔为20分钟
-- * 不传入MachineIdList及为汇总所有设备数据
-- ============================================= 
CREATE PROCEDURE [dbo].[sp_CurrentStateDurationStatistics](@MachineIdList NVARCHAR(MAX))
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @needCalculateMachineId NVARCHAR(MAX);
		DECLARE @updateDataMaxminute INT = 20;
		
		IF (@MachineIdList IS NOT NULL)
		    -- 获取需要更新数据的设备列表，包括
		    -- >> 当天还未汇总的
		    -- >> 汇总时间已超出最大汇总间隔
		    SET @needCalculateMachineId = (
		            SELECT DISTINCT CONVERT(NVARCHAR, fsi.Item) + ','
		            FROM   dbo.func_SplitInts(@MachineIdList, ',') AS fsi
		                   LEFT JOIN DailyStatesSummaries AS dss
		                        ON  (
		                                dss.MachineId = fsi.Item
		                                AND dss.ShiftDay >= DATEADD(DAY, -1, GETDATE())
		                            )
		            WHERE  (
		                       dss.LastModifyTime IS NULL
		                       OR (DATEDIFF(minute, dss.LastModifyTime, GETDATE())) 
		                          > @updateDataMaxminute
		                   ) FOR XML PATH('')
		        )
		ELSE
		    -- 标示，需要汇总所有数据
		    SET @needCalculateMachineId = NULL;
		
		
		IF (
		       LEN(@needCalculateMachineId) > 0
		       OR @needCalculateMachineId IS NULL
		   )
		BEGIN
		    --更新已经存在的数据
		    UPDATE DailyStatesSummaries
		    SET    DebugDuration             = T.DebugDuration,
		           OfflineDuration           = T.OfflineDuration,
		           FreeDuration              = T.FreeDuration,
		           RunDuration               = T.RunDuration,
		           StopDuration              = T.StopDuration,
		           TotalDuration             = T.TotalDuration,
		           shiftDay                  = T.shiftDay,
		           MachinesShiftDetailId     = T.MachinesShiftDetailId,
		           LastModifyTime            = GETDATE()
		    FROM   dbo.func_GetStateSummeryByDate(
		               CONVERT(NVARCHAR(20), GETDATE()-1, 23),
		               GETDATE(),
		               @needCalculateMachineId
		           ) AS T
		    WHERE  T.MachineId = DailyStatesSummaries.MachineId
		           AND T.ShiftDay = DailyStatesSummaries.ShiftDay
		           AND T.MachinesShiftDetailId=DailyStatesSummaries.MachinesShiftDetailId
		           AND (
		                   DailyStatesSummaries.LastModifyTime IS NULL
		                   OR (
		                          DATEDIFF(minute, DailyStatesSummaries.LastModifyTime, GETDATE())
		                      ) > @updateDataMaxminute
		               );
		    
		    
		    --Insert新数据
		    INSERT INTO DailyStatesSummaries
		      (
		        MachineId,
		        DebugDuration,
		        OfflineDuration,
		        FreeDuration,
		        RunDuration,
		        StopDuration,
		        TotalDuration,
		        LastModifyTime,
		        shiftDay,
		        MachinesShiftDetailId
		      )
		    SELECT MachineId,
		           DebugDuration,
		           OfflineDuration,
		           FreeDuration,
		           RunDuration,
		           StopDuration,
		           TotalDuration,
		           GETDATE(),
		           shiftDay,
		           MachinesShiftDetailId
		    FROM   dbo.func_GetStateSummeryByDate(
		               CONVERT(NVARCHAR(20), GETDATE(), 23),
		               GETDATE(),
		               @needCalculateMachineId
		           ) AS T
		    WHERE  NOT EXISTS (
		               SELECT 1
		               FROM   DailyStatesSummaries AS dss
		               WHERE  dss.MachineId = T.MachineId
		                      AND dss.ShiftDay = T.ShiftDay
		                      AND dss.MachinesShiftDetailId =T.MachinesShiftDetailId
		                      
		                     
		           )
		END
	END TRY
	BEGIN CATCH
		--把错误信息输出到错误日志表
		INSERT INTO dbo.ErrorLogs
		  (
		    Number,
		    Serverity,
		    STATE,
		    ProcName,
		    Line,
		    MESSAGE,
		    OccurDate,
		    InParams
		  )
		SELECT ERROR_NUMBER()    AS Number,
		       ERROR_SEVERITY()  AS Serverity,
		       ERROR_STATE()     AS STATE,
		       ISNULL(ERROR_PROCEDURE(), 'sp_CurrentStateDurationStatistics') AS 
		       ProcName,
		       ERROR_LINE() AS Line,
		       ERROR_MESSAGE() AS MESSAGE,
		       GETDATE() AS OccurDate,
		       '' AS InParams;
	END CATCH
END
GO

