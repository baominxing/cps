GO

/****** Object:  StoredProcedure [dbo].[sp_GetArchiveTargetTableByPartNo]    Script Date: 2021-05-10 16:12:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetArchiveTargetTableByPartNo]
    -- Add the parameters for the stored procedure here
    @PartNo NVARCHAR(100)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    -- 方法2：使用表变量
    -- 声明表变量
    DECLARE @temp TABLE
    (
        ArchivedTable NVARCHAR(100)
    );

    -- 将源表中的数据插入到表变量中
    INSERT INTO @temp
    (
        ArchivedTable
    )
    SELECT DISTINCT
           ArchivedTable
    FROM dbo.ArchiveEntries
    WHERE TargetTable = 'TraceFlowRecords'
    UNION
    (SELECT ArchivedTable = 'TraceFlowRecords')
    ORDER BY ArchivedTable DESC;

    -- 声明变量
    DECLARE @ArchivedTable AS NVARCHAR(100);
    DECLARE @tablename NVARCHAR(100);
    DECLARE @sql NVARCHAR(2000);
    DECLARE @count INT;
    SET @tablename = N'';
    SET @count = 0;

    WHILE EXISTS (SELECT ArchivedTable FROM @temp)
    BEGIN
        -- 也可以使用top 1
        SELECT TOP 1
               @ArchivedTable = ArchivedTable
        FROM @temp;

        PRINT @ArchivedTable;

        SET @sql = N'select @count = count(*) from [' + @ArchivedTable + N'] WHERE PartNo = @partno';

        EXEC sp_executesql @sql,
                           N'@partno NVARCHAR(100) ,@count int output',
                           @PartNo,
                           @count OUTPUT;
        PRINT @sql;

        IF @count > 0
        BEGIN
            PRINT 'break';
            SET @tablename = @ArchivedTable;
            BREAK;
        END;

        DELETE FROM @temp
        WHERE ArchivedTable = @ArchivedTable;
    END;

    SELECT @tablename;
END;
GO


