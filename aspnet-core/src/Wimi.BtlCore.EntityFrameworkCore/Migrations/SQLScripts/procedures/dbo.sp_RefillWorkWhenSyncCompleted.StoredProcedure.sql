/****** Object:  StoredProcedure [dbo].[sp_RefillWorkWhenSyncCompleted]    Script Date: 2020/2/19 14:52:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	更新States,Capacities表的结束时间
--              States,Capacities,Alarms表 更新SolutionName，ShiftDay,人和设备的ShiftName
-- Creator: leo
-- CreateTime: 2017-0328

-- * 由Hangfire 同步Mongo数据中调用
-- * 目前频率频率：web.Config中配置，默认5分钟
-- ============================================= 
CREATE PROCEDURE [dbo].[sp_RefillWorkWhenSyncCompleted]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @StartTime DATETIME;
	DECLARE @EndTime DATETIME;
	DECLARE @MasterId UNIQUEIDENTIFIER;
	DECLARE @ProcStartTime DATETIME;
	DECLARE @ProcEndTime DATETIME;
	
	DECLARE @SPName NVARCHAR = 'sp_RefillWorkWhenSyncCompleted';
	
	
	--设置此次master表的主键
	SET @MasterId = NEWID();
	SET @ProcStartTime = GETDATE();
	
	--更新States表的结束时间
	SET @StartTime = @ProcStartTime;
	
	BEGIN TRY
		UPDATE States
		SET    NAME = si.DisplayName
		FROM   StateInfos AS si
		WHERE  States.Code = si.Code
		       AND States.Name = ''
		
		UPDATE STATES
		SET    STATES.EndTime = A.EndTime,
		       STATES.LastModificationTime =GETDATE(),
		       STATES.duration = DATEDIFF(S, STATES.StartTime, A.EndTime)
		FROM   (
		           SELECT T2.[MachineId],
		                  T2.[StartTime],
		                  t1.EndTime,
		                  T2.Id
		           FROM   (
		                      SELECT ROW_NUMBER() OVER(PARTITION BY OS.[MachineId] ORDER BY OS.[StartTime] DESC) AS 
		                             rownum,
		                             os.[MachineId],
		                             OS.[StartTime],
		                             OS.Id
		                      FROM   STATES AS OS
		                      WHERE  OS.EndTime IS NULL
		                  ) T2
		                  INNER JOIN (
		                           SELECT ROW_NUMBER() OVER(PARTITION BY OS.[MachineId] ORDER BY OS.[StartTime] DESC) AS 
		                                  rownum,
		                                  os.[MachineId],
		                                  OS.[StartTime] AS EndTime
		                           FROM   STATES AS OS
		                           WHERE  OS.EndTime IS NULL
		                       ) T1
		                       ON  T1.[MachineId] = T2.[MachineId]
		                       AND T1.rownum + 1 = T2.rownum
		       ) AS A
		WHERE  STATES.[MachineId] = A.[MachineId]
		       AND STATES.Id = A.Id
		       AND STATES.EndTime IS NULL;
		
		--更新States表的 	ShiftDetail_SolutionName,ShiftDetail_StaffShiftName,ShiftDetail_MachineShiftName,ShiftDetail_ShiftDay   
		UPDATE s
		SET    s.ShiftDetail_SolutionName = ss.Name,
		       s.ShiftDetail_ShiftDay = msd2.ShiftDay,
		       s.ShiftDetail_StaffShiftName = ssi.Name,
		       s.ShiftDetail_MachineShiftName = ssi2.Name,
			   LastModificationTime = GETDATE()
		FROM   States AS s 
		       -- 员工
		       
		       LEFT JOIN MachinesShiftDetails AS msd
		            ON  msd.id = s.UserShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi
		            ON  ssi.Id = msd.ShiftSolutionItemId
		                -- 设备   
		                
		       LEFT JOIN MachinesShiftDetails AS msd2
		            ON  msd2.id = s.MachinesShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi2
		            ON  ssi2.Id = msd2.ShiftSolutionItemId
		       LEFT JOIN ShiftSolutions AS ss
		            ON  ss.Id = ssi2.ShiftSolutionId
		WHERE  s.ShiftDetail_SolutionName IS NULL
		       AND s.MachinesShiftDetailId > 0
		
		
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
		SELECT @MasterId            AS InfoLogId,
		       '更新States表完成'   AS Step,
		       ''                   AS MESSAGE,
		       @@ROWCOUNT - 1       AS AffectedRowCount,
		       @StartTime           AS StratTime,
		       @EndTime             AS EndTime,
		       DATEDIFF(SECOND, @StartTime, @EndTime) AS Duration;
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
		       ISNULL(ERROR_PROCEDURE(), @SPName) AS ProcName,
		       ERROR_LINE()      AS Line,
		       ERROR_MESSAGE()   AS MESSAGE,
		       GETDATE()         AS OccurDate,
		       ''                AS InParams;
	END CATCH
	
	
	
	--更新Capacity表的结束时间
	SET @StartTime = GETDATE();
	
	BEGIN TRY
		UPDATE Capacities
		SET    Capacities.EndTime = A.EndTime,
		       Capacities.duration = DATEDIFF(S, Capacities.StartTime, A.EndTime)
		FROM   (
		           SELECT T2.[MachineId],
		                  T2.[StartTime],
		                  t1.EndTime,
		                  T2.Id
		           FROM   (
		                      SELECT ROW_NUMBER() OVER(PARTITION BY OS.[MachineId] ORDER BY OS.[StartTime] DESC) AS 
		                             rownum,
		                             os.[MachineId],
		                             OS.[StartTime],
		                             OS.Id
		                      FROM   Capacities AS OS
		                      WHERE  OS.EndTime IS NULL
		                  ) T2
		                  INNER JOIN (
		                           SELECT ROW_NUMBER() OVER(PARTITION BY OS.[MachineId] ORDER BY OS.[StartTime] DESC) AS 
		                                  rownum,
		                                  os.[MachineId],
		                                  OS.[StartTime] AS EndTime
		                           FROM   Capacities AS OS
		                           WHERE  OS.EndTime IS NULL
		                       ) T1
		                       ON  T1.[MachineId] = T2.[MachineId]
		                       AND T1.rownum + 1 = T2.rownum
		       ) AS A
		WHERE  Capacities.[MachineId] = A.[MachineId]
		       AND Capacities.Id=A.Id
		       AND Capacities.EndTime IS NULL;
		
		--根据状态表修正产量结束时间
		--修正产量结束时间,视断线时间点为产量结束时间
		UPDATE Capacities
		SET    Capacities.EndTime = A.Endtime,
		       Capacities.Duration = DATEDIFF(S, Capacities.StartTime, A.EndTime)
		FROM   (
		           SELECT cc.MachineId,
		                  cc.StartTime,
		                  ss.StartTime Endtime
		           FROM   Capacities cc
		                  INNER JOIN [States] AS ss
		                       ON  Ss.MachineId = cc.MachineId
		                       AND ss.StartTime > cc.StartTime
		                       AND ss.StartTime < cc.EndTime
		           WHERE  ss.Code = 'Offline'
		                  AND ss.StartTime > DATEADD(DAY, -2, GETDATE())
		       ) A
		WHERE  Capacities.MachineId = a.MachineId
		       AND Capacities.StartTime = a.StartTime;
		
		--更新Capacities表的 	ShiftDetail_SolutionName,ShiftDetail_StaffShiftName,ShiftDetail_MachineShiftName,ShiftDetail_ShiftDay        
		UPDATE c
		SET    c.ShiftDetail_SolutionName = ss.Name,
		       c.ShiftDetail_ShiftDay = msd2.ShiftDay,
		       c.ShiftDetail_StaffShiftName = ssi.Name,
		       c.ShiftDetail_MachineShiftName = ssi2.Name
		FROM   Capacities c
		       -- 员工
		       
		       LEFT JOIN MachinesShiftDetails AS msd
		            ON  msd.id = c.UserShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi
		            ON  ssi.Id = msd.ShiftSolutionItemId
		                -- 设备   
		                
		       LEFT JOIN MachinesShiftDetails AS msd2
		            ON  msd2.id = c.MachinesShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi2
		            ON  ssi2.Id = msd2.ShiftSolutionItemId
		       LEFT JOIN ShiftSolutions AS ss
		            ON  ss.Id = ssi2.ShiftSolutionId
		WHERE  c.ShiftDetail_SolutionName IS NULL
		       AND c.MachinesShiftDetailId > 0
		
		
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
		SELECT @MasterId                AS InfoLogId,
		       '更新Capacities表完成'   AS Step,
		       ''                       AS MESSAGE,
		       @@ROWCOUNT - 1           AS AffectedRowCount,
		       @StartTime               AS StratTime,
		       @EndTime                 AS EndTime,
		       DATEDIFF(SECOND, @StartTime, @EndTime) AS Duration;
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
		       ISNULL(ERROR_PROCEDURE(), @SPName) AS ProcName,
		       ERROR_LINE()      AS Line,
		       ERROR_MESSAGE()   AS MESSAGE,
		       GETDATE()         AS OccurDate,
		       ''                AS InParams;
	END CATCH
	
	BEGIN TRY
		--更新Alarms表的 	ShiftDetail_SolutionName,ShiftDetail_StaffShiftName,ShiftDetail_MachineShiftName,ShiftDetail_ShiftDay        
		UPDATE a
		SET    a.ShiftDetail_SolutionName = ss.Name,
		       a.ShiftDetail_ShiftDay = msd2.ShiftDay,
		       a.ShiftDetail_StaffShiftName = ssi.Name,
		       a.ShiftDetail_MachineShiftName = ssi2.Name
		FROM   Alarms AS a 
		       -- 员工
		       
		       LEFT JOIN MachinesShiftDetails AS msd
		            ON  msd.id = a.UserShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi
		            ON  ssi.Id = msd.ShiftSolutionItemId
		                -- 设备      
		                
		       LEFT JOIN MachinesShiftDetails AS msd2
		            ON  msd2.id = a.MachinesShiftDetailId
		       LEFT JOIN ShiftSolutionItems AS ssi2
		            ON  ssi2.Id = msd2.ShiftSolutionItemId
		       LEFT JOIN ShiftSolutions AS ss
		            ON  ss.Id = ssi2.ShiftSolutionId
		WHERE  a.ShiftDetail_SolutionName IS NULL
		       AND a.MachinesShiftDetailId > 0
		
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
		SELECT @MasterId            AS InfoLogId,
		       '更新Alarms表完成'   AS Step,
		       ''                   AS MESSAGE,
		       @@ROWCOUNT - 1       AS AffectedRowCount,
		       @StartTime           AS StratTime,
		       @EndTime             AS EndTime,
		       DATEDIFF(SECOND, @StartTime, @EndTime) AS Duration;
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
		       ISNULL(ERROR_PROCEDURE(), @SPName) AS ProcName,
		       ERROR_LINE()      AS Line,
		       ERROR_MESSAGE()   AS MESSAGE,
		       GETDATE()         AS OccurDate,
		       ''                AS InParams;
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
	       @SPName         AS ProcName,
	       @ProcStartTime  AS StratTime,
	       @ProcEndTime    AS EndTime,
	       DATEDIFF(SECOND, @ProcStartTime, @ProcEndTime) AS Duration
END
GO

