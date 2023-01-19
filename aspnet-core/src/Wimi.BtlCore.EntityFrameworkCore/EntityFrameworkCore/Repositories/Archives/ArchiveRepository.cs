using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Archives.Repository;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Archives
{
    public class ArchiveRepository : IArchiveRepository
    {
        private readonly string tempTableName = "@temp";
        private readonly string separator = ",";
        private readonly int Limit = 10000;

        public bool CheckArchiveTableIsExisted(ArchiveEntry archiveEntry)
        {
            var list = new List<dynamic>();

            var executeSql = $@"
SELECT TOP 1 *
FROM sysobjects
WHERE type = 'U'
      AND name = @ArchivedTable;
";

            var parameters = new { ArchivedTable = archiveEntry.ArchivedTable };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                list = conn.Query<dynamic>(executeSql, parameters).ToList();
            }

            return list.Any();
        }

        public void CreateArchiveTable(ArchiveEntry archiveEntry)
        {
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                // 初始化一个连接
                Server server = new Server(new ServerConnection(conn));
                //得到数据库
                var srcDb = server.Databases[conn.Database];
                //得到表
                Table table = srcDb.Tables[archiveEntry.TargetTable];

                //初始化Scripter 
                Scripter scripter = new Scripter(server);

                scripter.Options.Add(ScriptOption.DriAllConstraints);
                scripter.Options.Add(ScriptOption.DriAllKeys);
                scripter.Options.Add(ScriptOption.Default);
                scripter.Options.Add(ScriptOption.ContinueScriptingOnError);
                scripter.Options.Add(ScriptOption.ConvertUserDefinedDataTypesToBaseType);
                scripter.Options.Add(ScriptOption.Indexes);

                UrnCollection collection = new UrnCollection();

                collection.Add(table.Urn);

                var createTableSql = new StringBuilder();

                var sqls = scripter.Script(collection);

                foreach (var s in sqls)
                {
                    createTableSql.AppendLine(s.Replace(archiveEntry.TargetTable, archiveEntry.ArchivedTable));
                }

                srcDb.ExecuteNonQuery(createTableSql.ToString());
            }
        }

        public void DeleteDataFromTargetTable(ArchiveEntry archiveEntry, List<string> archiveDatas)
        {
            var executeSql = $@"
{archiveDatas.ToSQLServerTable(separator, tempTableName)}
DELETE dbo.{archiveEntry.TargetTable} FROM dbo.{archiveEntry.TargetTable} AS T1 JOIN {tempTableName} AS T2 ON T1.Id = t2.F1
";
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Execute(executeSql);
            }
        }

        /// <summary>
        /// 获取归档数据,限制每次归档最多1w笔记录,超过的则放到下次,可以根据历史记录进行调节
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="archiveEntry"></param>
        /// <returns></returns>
        public List<string> GetArchiveDatas(string startTime, ArchiveEntry archiveEntry)
        {
            var joinColumnName = archiveEntry.TargetTable == "TraceCatalogs" ? "MachineShiftDetailId" : "MachinesShiftDetailId";

            var list = new List<string>();

            var executeSql = $@"
SELECT TOP {Limit} t1.Id
FROM {archiveEntry.TargetTable} AS t1
    JOIN MachinesShiftDetails AS t2
        ON t1.{joinColumnName} = t2.Id
WHERE t2.{archiveEntry.ArchiveColumn} BETWEEN @StartTime AND @ArchiveValue
";

            var parameters = new { StartTime = startTime, ArchiveValue = archiveEntry.ArchiveValue };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                list = conn.Query<string>(executeSql, parameters).ToList();
            }

            return list;
        }

        /// <summary>
        /// 获取归档数据，限制每次归档最多1w笔记录，超过的则放到下次，可以根据历史记录进行调节
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="archiveEntry"></param>
        /// <returns></returns>
        public Tuple<List<long>, List<long>> GetArchiveDatasTraceCatalogs(string startTime, ArchiveEntry archiveEntry)
        {
            var list = new List<dynamic>();

            var executeSql = $@"
SELECT TOP {Limit} t0.Id TraceFlowRecordId,t1.Id TraceCatalogId
FROM dbo.TraceFlowRecords AS t0
    JOIN dbo.TraceCatalogs AS t1
        ON t0.PartNo = t1.PartNo
    JOIN MachinesShiftDetails AS t2
        ON t1.MachineShiftDetailId = t2.Id
WHERE t2.{archiveEntry.ArchiveColumn} BETWEEN @StartTime AND @ArchiveValue
";

            var parameters = new { StartTime = startTime, ArchiveValue = archiveEntry.ArchiveValue };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                list = conn.Query<dynamic>(executeSql, parameters).ToList();
            }

            var traceCatalogIds = list.Select(s => (long)s.TraceCatalogId).Distinct().ToList();
            var traceFlowRecordIds = list.Select(s => (long)s.TraceFlowRecordId).Distinct().ToList();

            var traceRecords = Tuple.Create(traceCatalogIds, traceFlowRecordIds);

            return traceRecords;
        }

        public List<string> GetArchiveTableColumns(ArchiveEntry archiveEntry)
        {
            var list = new List<string>();

            var executeSql = $@"
SELECT '['+c.name+']'
FROM sysobjects AS o
    JOIN sys.columns AS c
        ON o.id = c.object_id
WHERE type = 'U'
      AND o.name = @TargetTable
";

            var parameters = new { TargetTable = archiveEntry.TargetTable };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                list = conn.Query<string>(executeSql, parameters).ToList();
            }

            return list;
        }

        public void InsertDataToArchiveTable(ArchiveEntry archiveEntry, List<string> archiveDatas, List<string> columns)
        {
            var executeSql = $@"
{archiveDatas.ToSQLServerTable(separator, tempTableName)}
SET IDENTITY_INSERT [{archiveEntry.ArchivedTable}] ON;
INSERT INTO [{archiveEntry.ArchivedTable}]({string.Join(",", columns).TrimEnd(',')})
SELECT {string.Join(",", columns).TrimEnd(',').Replace("[ArchivedTable]", "'" + archiveEntry.ArchivedTable + "'")}
FROM dbo.{archiveEntry.TargetTable} AS T1 JOIN {tempTableName} T2 ON T1.Id = t2.F1
SET IDENTITY_INSERT [{archiveEntry.ArchivedTable}] OFF;
";
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Execute(executeSql);
            }
        }
    }
}
