 
CREATE PROC [dbo].[sp_ListMachineStates]
  @StartTime DATETIME ,
  @EndTime DATETIME  
AS 

BEGIN
	
	
	if object_id(N'#NSTATES',N'U') is not null
 DROP   TABLE #NSTATES
 
	if object_id(N'#NREASONS',N'U') is not null
DROP   TABLE #NREASONS

	if object_id(N'#TORI',N'U') is not null
DROP   TABLE #TORI

	if object_id(N'#TOTAL',N'U') is not null
DROP   TABLE #TOTAL

	if object_id(N'#TTR',N'U') is not null
DROP TABLE #TTR



			SELECT 
				   [Id]
				  ,[Code]
				  ,[DateKey]
				  ,CASE WHEN [EndTime] IS NULL THEN GETDATE() ELSE [EndTime] END EndTime
				  ,[MachineCode]
				  ,[MachineId]
				  ,[MachinesShiftDetailId]
				  ,[Memo]
				  ,[Name]
				  ,[OrderId]
				  ,[PartNo]
				  ,[ProductId]
				  ,[ProcessId]
				  ,[ProgramName]
				  ,[ShiftDetail_MachineShiftName]
				  ,[ShiftDetail_ShiftDay]
				  ,[ShiftDetail_SolutionName]
				  ,[ShiftDetail_StaffShiftName]
				  ,[StartTime]
				  ,[UserId]
				  ,[UserShiftDetailId] INTO #NSTATES
			 FROM STATES
			 WHERE [StartTime] >= @StartTime AND  [StartTime]< @EndTime 
	 
 			SELECT 
				   RFR.[MachineId]
				  ,RFR.[StateId]
				  ,RFR.[StateCode]
				  ,RFR.[StartTime]
				  ,CASE WHEN RFR.[EndTime] IS NULL THEN GETDATE() ELSE RFR.[EndTime] END EndTime
				  ,RFR.[EndUserId]
				  ,SI.DisplayName RName INTO #NREASONS
			 FROM ReasonFeedbackRecords RFR
			 INNER JOIN StateInfos SI ON RFR.StateId = SI.Id
			 
 
 
			SELECT 
				   S.Id
				  ,S.Code
				  ,S.DateKey
				  ,S.EndTime
				  ,S.MachineCode
				  ,S.MachineId
				  ,S.MachinesShiftDetailId
				  ,S.Memo
				  ,S.Name
				  ,S.OrderId
				  ,S.PartNo
				  ,S.ProductId
				  ,S.ProcessId
				  ,S.ProgramName
				  ,S.ShiftDetail_MachineShiftName
				  ,S.ShiftDetail_ShiftDay
				  ,S.ShiftDetail_SolutionName
				  ,S.ShiftDetail_StaffShiftName
				  ,S.StartTime
				  ,S.UserId
				  ,S.UserShiftDetailId
				  ,R.StateCode
				  ,R.StartTime RStartTime
				  ,R.EndTime REndTime
				  ,R.RName
				  ,ROW_NUMBER() OVER(PARTITION BY S.ID ORDER BY R.[StartTime]) AS rownum 
				  INTO #TORI
			FROM #NSTATES S
			LEFT JOIN #NREASONS R ON (S.MachineId =R.MachineId AND (NOT ((s.EndTime < R.StartTime) OR (s.StartTime > r.EndTime))) )
 

 
			SELECT
				   T1.Code
				  ,T1.DateKey
				  ,CASE WHEN T3.Id IS NULL THEN T1.ENDTIME ELSE T1.REndTime END EndTime
				  ,T1.MachineCode
				  ,T1.MachineId
				  ,T1.MachinesShiftDetailId
				  ,T1.Memo
				  ,T1.Name
				  ,T1.OrderId
				  ,T1.PartNo
				  ,T1.ProductId
				  ,T1.ProcessId
				  ,T1.ProgramName
				  ,T1.ShiftDetail_MachineShiftName
				  ,T1.ShiftDetail_ShiftDay
				  ,T1.ShiftDetail_SolutionName
				  ,T1.ShiftDetail_StaffShiftName
				  ,CASE WHEN T2.Id IS NULL THEN T1.StartTime ELSE T2.REndTime END AS StartTime
				  ,T1.UserId
				  ,T1.UserShiftDetailId
				  ,T1.StateCode
				  ,T1.RStartTime
				  ,T1.REndTime
				  ,T1.RName
				  INTO  #TTR
			FROM #TORI T1
			LEFT JOIN #TORI T2 ON (T1.Id = T2.Id AND T1.rownum-1 = T2.rownum)
			LEFT JOIN #TORI T3 ON (T1.Id = T3.Id AND T1.rownum = T3.rownum-1)
 

 
			SELECT TTR.*
				  ,CASE WHEN TTR.RStartTime IS NULL OR TTR.RStartTime >= TTR.EndTime OR TTR.REndTime <= TTR.StartTime THEN 'NONE'
						WHEN TTR.RStartTime<=TTR.StartTime AND TTR.REndTime>= TTR.EndTime THEN 'OUT'
						WHEN TTR.RStartTime<=TTR.StartTime AND TTR.REndTime>TTR.StartTime AND TTR.REndTime<TTR.EndTime THEN 'LEFT'
						WHEN TTR.RStartTime>TTR.StartTime AND TTR.RStartTime<TTR.EndTime AND TTR.REndTime>=TTR.EndTime THEN 'RIGHT'
						WHEN TTR.RStartTime>TTR.StartTime AND TTR.RStartTime<TTR.EndTime AND TTR.REndTime>TTR.StartTime AND TTR.REndTime<TTR.EndTime THEN 'IN'
						ELSE 'IMPOSSIBLE' END STATETYPE
						INTO #TOTAL
			FROM #TTR  AS TTR
 

		SELECT T.MachineId,T.StartTime,T.EndTime,T.Code,T.Name,DATEDIFF(SECOND,T.StartTime,T.EndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM  #TOTAL T
		WHERE T.STATETYPE = 'NONE'

		UNION
		SELECT T.MachineId,T.StartTime,T.EndTime,T.StateCode Code,T.RName Name,DATEDIFF(SECOND,T.StartTime,T.EndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'OUT'

		UNION
		SELECT T.MachineId,T.StartTime,T.REndTime EndTime,T.StateCode Code,T.RName Name,DATEDIFF(SECOND,T.StartTime,T.REndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'LEFT'

		UNION
		SELECT T.MachineId,T.REndTime StartTime,T.EndTime,T.Code,T.Name,DATEDIFF(SECOND,T.REndTime,T.EndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'LEFT'

		UNION
		SELECT T.MachineId,T.StartTime,T.RStartTime EndTime,T.Code,T.Name,DATEDIFF(SECOND,T.StartTime,T.RStartTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'RIGHT'

		UNION
		SELECT T.MachineId,T.RStartTime StartTime,T.EndTime,T.StateCode Code,T.RName Name,DATEDIFF(SECOND,T.RStartTime,T.EndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'RIGHT'

		UNION
		SELECT T.MachineId,T.StartTime,T.RStartTime EndTime,T.Code,T.Name,DATEDIFF(SECOND,T.StartTime,T.RStartTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'IN'

		UNION
		SELECT T.MachineId,T.RStartTime StartTime,T.REndTime EndTime,T.StateCode Code,T.RName Name,DATEDIFF(SECOND,T.RStartTime,T.REndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'IN'

		UNION
		SELECT T.MachineId,T.REndTime StartTime,T.EndTime,T.Code,T.Name,DATEDIFF(SECOND,T.REndTime,T.EndTime) Duration
			  ,T.DateKey
			  ,T.MachineCode
			  ,T.MachinesShiftDetailId
			  ,T.Memo
			  ,T.OrderId
			  ,T.PartNo
			  ,T.ProductId
			  ,T.ProcessId
			  ,T.ProgramName
			  ,T.ShiftDetail_MachineShiftName
			  ,T.ShiftDetail_ShiftDay
			  ,T.ShiftDetail_SolutionName
			  ,T.ShiftDetail_StaffShiftName
			  ,T.UserId
			  ,T.UserShiftDetailId
		FROM #TOTAL T
		WHERE T.STATETYPE = 'IN'
 
 END