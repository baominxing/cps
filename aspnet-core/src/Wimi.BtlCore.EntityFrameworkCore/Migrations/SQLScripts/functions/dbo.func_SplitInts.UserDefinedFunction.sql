/****** Object:  UserDefinedFunction [dbo].[func_SplitInts]    Script Date: 2020/2/19 14:27:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:
-- 用以按指定的分隔符拆分传入的设备Id列表以便于SQL查询
-- Creator:chengjie.zhang
-- CreateTime:
-- ============================================= 
CREATE FUNCTION [dbo].[func_SplitInts]
(
	@List          VARCHAR(MAX),
	@Delimiter     VARCHAR(255)
)
RETURNS TABLE
AS

  RETURN (
             SELECT Item = CONVERT(INT, Item)
             FROM   (
                        SELECT Item = x.i.value('(./text())[1]', 'varchar(max)')
                        FROM   (
                                   SELECT [XML] = CONVERT(
                                              XML,
                                              '<i>'
                                              + REPLACE(@List, @Delimiter, '</i><i>') 
                                              + '</i>'
                                          ).query('.')
                               ) AS a
                               CROSS APPLY [XML].nodes('i') AS x(i)
                    ) AS y
             WHERE  Item IS NOT NULL
         );
GO

