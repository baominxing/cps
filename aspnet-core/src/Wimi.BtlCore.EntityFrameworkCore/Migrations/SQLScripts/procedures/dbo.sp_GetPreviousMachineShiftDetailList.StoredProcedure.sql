/****** Object:  StoredProcedure [dbo].[sp_GetPreviousMachineShiftDetailList]    Script Date: 2020/2/19 14:50:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	获取前一个班次
-- Creator: fred.bao
-- CreateTime: 

-- * Todo 过滤设备权限
-- ============================================= 

CREATE PROCEDURE [dbo].[sp_GetPreviousMachineShiftDetailList] (@MachineIdListString NVARCHAR(MAX))
AS
BEGIN
	SET NOCOUNT ON;
	-- 避免多个存储过程使用同一个零时表加存储过程大写首字母后缀
	IF OBJECT_ID('tempdb..#tempGMA') IS NOT NULL
	    DROP TABLE #tempGMA
	
	IF OBJECT_ID('tempdb..#tempGMA2') IS NOT NULL
	    DROP TABLE #tempGMA2
	    
	CREATE TABLE #tempGMA2 (PreviousShiftDetailId INT)
	--当前班次Id
	DECLARE @Id INT 
	--设备Id
	DECLARE @MachineId INT
	--设备当前班次的前一个班次Id
	DECLARE @PreviousShiftDetailId INT
	
	--获得当前设备班次号
	SELECT m.Id,
	       m.MachineId
	INTO   #tempGMA
	FROM   (
	           SELECT msd.Id,
	                  msd.MachineId
	           FROM   MachinesShiftDetails AS msd
	                  JOIN ShiftSolutionItems AS ssi
	                       ON  msd.ShiftSolutionItemId = ssi.Id
	                  JOIN Calendars AS c
	                       ON  c.[Date] = msd.ShiftDay
	           WHERE  msd.MachineId IN (SELECT fs.Item
	                                    FROM   [func_SplitInts](@MachineIdListString, ',') AS 
	                                           fs)
	                  AND GETDATE() BETWEEN CONVERT(NVARCHAR(10), msd.ShiftDay, 23) 
	                      +
	                      ' ' +
	                      CONVERT(VARCHAR(8), ssi.StartTime, 108)
	                      AND CASE 
	                               WHEN ssi.StartTime > ssi.EndTime THEN DATEADD(
	                                        DAY,
	                                        1,
	                                        CONVERT(NVARCHAR(10), msd.ShiftDay, 23) 
	                                        +
	                                        ' '
	                                        + CONVERT(VARCHAR(8), ssi.EndTime, 108)
	                                    )
	                               ELSE CONVERT(NVARCHAR(10), msd.ShiftDay, 23) 
	                                    +
	                                    ' '
	                                    + CONVERT(VARCHAR(8), ssi.EndTime, 108)
	                          END
	       ) AS m
	
	--循环遍历临时表
	WHILE EXISTS (
	          SELECT Id,
	                 MachineId
	          FROM   #tempGMA
	      )
	BEGIN
	    SELECT TOP 1 @Id = Id,
	           @MachineId = MachineId
	    FROM   #tempGMA 
	    
	    DELETE 
	    FROM   #tempGMA
	    WHERE  Id = @Id
	    
	     --获得当前设备的前一个班次Id
	    SELECT TOP 1 @PreviousShiftDetailId = msd.Id
	    FROM   MachinesShiftDetails AS msd
	    WHERE  msd.Id < @Id
	           AND msd.MachineId = @MachineId
	    ORDER BY
	           msd.Id DESC
	           
	    INSERT INTO #tempGMA2(PreviousShiftDetailId) VALUES (@PreviousShiftDetailId)
	    
	    SET @PreviousShiftDetailId = NULL
	    
	END
	SELECT *
	FROM   #tempGMA2;
END
GO


