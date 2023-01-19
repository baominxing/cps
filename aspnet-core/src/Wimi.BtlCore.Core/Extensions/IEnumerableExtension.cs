using System.Collections.Generic;

namespace Wimi.BtlCore.Extensions
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 将List类型的对象串拼成一个table与目标table可以用join语法
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="targetEnum"></param>
        /// <returns></returns>
        public static string ToSQLServerTable<T>(this IEnumerable<T> sourceList, string separator = ",", string tempTableName = "@temp") where T : class
        {
            var sourceString = string.Join(separator, sourceList);

            return $@"
DECLARE @i INT;
DECLARE @SourceSql NVARCHAR(MAX);
DECLARE @Separator NVARCHAR(100);
DECLARE {tempTableName} TABLE
(
    F1 VARCHAR(100)
);

SET @SourceSql = '{sourceString}';
SET @Separator = '{separator}';
SET @SourceSql = RTRIM(LTRIM(@SourceSql));
SET @i = CHARINDEX(@Separator, @SourceSql);

WHILE @i >= 1
BEGIN
    INSERT {tempTableName}
    VALUES
    (LEFT(@SourceSql, @i - 1));
    SET @SourceSql = SUBSTRING(@SourceSql, @i + 1, LEN(@SourceSql) - @i);
    SET @i = CHARINDEX(@Separator, @SourceSql);
END;

IF @SourceSql <> ''
    INSERT {tempTableName}
    VALUES
    (@SourceSql);

";
        }

    }

}
