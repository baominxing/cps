using Abp.Domain.Repositories;
using Abp.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BackgroundJobs.Process
{
    public class BasicProcess : BtlCoreAppServiceBase
    {
        private readonly IRepository<SyncDataFlag> syncDataFlagRepository;

        public BasicProcess(IRepository<SyncDataFlag> syncDataFlagRepository)
        {
            this.syncDataFlagRepository = syncDataFlagRepository;
        }

        protected void InsertIntoSqlServer(DataTable dt, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string targetName, string lastSyncTime)
        {
            if (dt.Rows.Count <= 0) return;

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();

                using (var bulkCopy = new SqlBulkCopy(conn))
                {
                    // bulkCopy.BatchSize = 10;
                    bulkCopy.DestinationTableName = targetName;
                    foreach (var item in columnMappings)
                    {
                        bulkCopy.ColumnMappings.Add(item);
                    }

                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        UpdateLastSyncDateTime(dt.Rows.Count, lastSyncTime);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Logger.Fatal($"ProcessName: {this.GetType().Name} ,Sql Bulk Copy Error,Message:{ex}");
                    }

                }
            }

            dt.Clear();
        }

        private void UpdateLastSyncDateTime(int count, string lastSyncTime)
        {
            var processName = this.GetType().BaseType?.Name;
            var entity = new SyncDataFlag
            {
                ProcessName = processName,
                LastSyncTime = lastSyncTime,
                LastSyncCount = count,
                TotalCount = count
            };

            var flag = this.syncDataFlagRepository.FirstOrDefault(n => n.ProcessName.Equals(processName));
            if (flag == null)
            {
                this.syncDataFlagRepository.Insert(entity);
            }
            else
            {
                flag.LastSyncCount = count;
                flag.LastSyncTime = lastSyncTime;
                flag.TotalCount = flag.TotalCount + count;
                this.syncDataFlagRepository.Update(flag);
            }
        }
    }
}