/****** Object:  StoredProcedure [dbo].[sp_StateDurationStatistics]    Script Date: 2020/2/19 14:51:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	统计前一天设备状态
-- Creator: fred.bao
-- CreateTime: 
-- * 通过job执行
-- * 目前频率：每天0点执行
-- * 更新：去除IsByShift和SummaryDate 字段
-- * 更新：重复运行时，删除原有重复数据
-- ============================================= 
CREATE PROCEDURE [dbo].[sp_StateDurationStatistics]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @StartTime DATETIME;
	DECLARE @EndTime DATETIME;
	DECLARE @MasterId UNIQUEIDENTIFIER;
	DECLARE @ProcStartTime DATETIME;
	DECLARE @ProcEndTime DATETIME;
	
	--设置此次master表的主键
	SET @MasterId = NEWID();
	SET @ProcStartTime = GETDATE();
	
	SET @StartTime = DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), -1);
	SET @EndTime = GETDATE();
	
	
	
	--统计前一天的所有状态的持续时间
	BEGIN TRY
		--删除之前的及时数据（确保会将重复数据全部删掉）
		DELETE d
		FROM   DailyStatesSummaries d
		       INNER JOIN (
		                SELECT f.ShiftDay,
		                       f.MachineId,
		                       f.MachinesShiftDetailId
		                FROM   dbo.func_GetStateSummeryByDate(@StartTime, @EndTime, NULL) 
		                       f
		            ) f
		            ON  d.ShiftDay = f.ShiftDay
		            AND d.MachineId = f.MachineId
		            AND d.MachinesShiftDetailId = f.MachinesShiftDetailId
		
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
		    ShiftDay,
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
		FROM   dbo.func_GetStateSummeryByDate(@StartTime, @EndTime, NULL);
		
		SET @EndTime = GETDATE();
		INSERT INTO dbo.InfoLogDetails
		  (
		    InfoLogId,
		    Step,
		    MESSAGE,
		    AffectedRowCount,
		    StratTime,
		    EndTime,
		    Duration
		  )
		SELECT @MasterId       AS InfoLogId,
		       '统计前一天的所有状态的持续时间' AS Step,
		       ''              AS MESSAGE,
		       @@ROWCOUNT      AS AffectedRowCount,
		       @ProcStartTime  AS StratTime,
		       @EndTime        AS EndTime,
		       DATEDIFF(SECOND, @ProcStartTime, @EndTime) AS Duration;
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
		       ISNULL(ERROR_PROCEDURE(), '统计前一天的所有状态的持续时间') AS 
		       ProcName,
		       ERROR_LINE() AS Line,
		       ERROR_MESSAGE() AS MESSAGE,
		       GETDATE() AS OccurDate,
		       '' AS InParams;
	END CATCH
	
	--记录存储过程执行明细记录
	SET @ProcEndTime = GETDATE();
	
	INSERT INTO dbo.InfoLogs
	  (
	    Id,
	    ProcName,
	    StartTime,
	    EndTime,
	    Duration
	  )
	SELECT @MasterId       AS Id,
	       '统计前一天的所有状态的持续时间' AS ProcName,
	       @ProcStartTime  AS StratTime,
	       @ProcEndTime    AS EndTime,
	       DATEDIFF(SECOND, @ProcStartTime, @ProcEndTime) AS Duration
END
GO

