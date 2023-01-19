using Abp.Domain.Repositories;
using Abp.Logging;

namespace Wimi.BtlCore.BackgroundJobs.Process
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Extensions;
    using Wimi.BtlCore.States.Mongo;

    public class StateProcess : BasicProcess
    {
        private readonly MongoStateManager mongoStateManager;
        private readonly IStateRepository stateRepository;

        public StateProcess(IRepository<SyncDataFlag> syncDataFlagRepository,
           MongoStateManager mongoStateManager, IStateRepository stateRepository) : base(syncDataFlagRepository)
        {
            this.mongoStateManager = mongoStateManager;
            this.stateRepository = stateRepository;
        }

        /// <summary>
        /// 处理流程
        /// </summary>
        public bool Process(string mongoTableName, string targetTable)
        {
            try
            {
                // 1.获取最新更新时间
                var lastSyncDateTime = this.stateRepository.GetMaxSyncStateDateTime();
                this.Logger.Info($"[{this.GetType().Name}], 获取最新同步时间:【{lastSyncDateTime}】");

                // 2.根据取得的时间段从Mongo中取得数据
                var source = this.GetMongoData(lastSyncDateTime);

                this.Logger.Info($"[{this.GetType().Name}], 从Mongo获取到数据:【{source.Count}】 条");

                if (!source.Any())
                {
                    return false;
                }

                // 3.创建用于暂存数据的DataTable
                var dataTable = BuildDataTable(mongoTableName, source);

                // 4.把数据搬运到SQL Server中
                var columnMappings = GetSqlBulkCopyColumnMappings();
                var syncTime = source.Last().CreationTime;
                this.InsertIntoSqlServer(dataTable, columnMappings, targetTable, syncTime);

                this.Logger.Info($"[{this.GetType().Name}], 更新最后索引:【{syncTime}】 ");
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal($"[Job] State Sync Error,Message:{ex}");
                return false;
            }

            return true;
        }

        private static DataTable InitDataTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("MachineId", typeof(long));
            dt.Columns.Add("MachineCode", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("StartTime", typeof(DateTime));
            dt.Columns.Add("EndTime", typeof(DateTime));
            dt.Columns.Add("Duration", typeof(decimal));
            dt.Columns.Add("Memo", typeof(string));
            dt.Columns.Add("UserId", typeof(int));
            dt.Columns.Add("UserShiftDetailId", typeof(int));
            dt.Columns.Add("MachinesShiftDetailId", typeof(int));
            dt.Columns.Add("OrderId", typeof(int));
            dt.Columns.Add("ProcessId", typeof(int));
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("PartNo", typeof(string));
            dt.Columns.Add("DateKey", typeof(int));
            dt.Columns.Add("ProgramName", typeof(string));
            dt.Columns.Add("IsShiftSplit", typeof(bool));
            dt.Columns.Add("CreationTime", typeof(DateTime));
            dt.Columns.Add("MongoCreationTime", typeof(string));
            dt.Columns.Add("ShiftDay", typeof(string));
            dt.Columns.Add("MachineShiftItemName", typeof(string));
            dt.Columns.Add("StaffShiftItemName", typeof(string));
            dt.Columns.Add("ShiftSolutionName", typeof(string));
            dt.Columns.Add("DmpId", typeof(Guid));
            dt.Columns.Add("PreviousLinkId", typeof(Guid));

            return dt;
        }


        private static IEnumerable<SqlBulkCopyColumnMapping> GetSqlBulkCopyColumnMappings()
        {
            return new List<SqlBulkCopyColumnMapping>
            {
                new SqlBulkCopyColumnMapping("MachineId", "MachineId"),
                new SqlBulkCopyColumnMapping("MachineCode", "MachineCode"),
                new SqlBulkCopyColumnMapping("Code", "Code"),
                new SqlBulkCopyColumnMapping("Name", "Name"),
                new SqlBulkCopyColumnMapping("StartTime", "StartTime"),
                new SqlBulkCopyColumnMapping("EndTime", "EndTime"),
                new SqlBulkCopyColumnMapping("Duration", "Duration"),
                new SqlBulkCopyColumnMapping("Memo", "Memo"),
                new SqlBulkCopyColumnMapping("UserId", "UserId"),
                new SqlBulkCopyColumnMapping("UserShiftDetailId","UserShiftDetailId"),
                new SqlBulkCopyColumnMapping( "MachinesShiftDetailId","MachinesShiftDetailId"),
                new SqlBulkCopyColumnMapping("OrderId", "OrderId"),
                new SqlBulkCopyColumnMapping("ProcessId", "ProcessId"),
                new SqlBulkCopyColumnMapping("ProductId", "ProductId"),
                new SqlBulkCopyColumnMapping("PartNo", "PartNo"),
                new SqlBulkCopyColumnMapping("DateKey", "DateKey"),
                new SqlBulkCopyColumnMapping("ProgramName", "ProgramName"),
                new SqlBulkCopyColumnMapping("IsShiftSplit","IsShiftSplit"),
                new SqlBulkCopyColumnMapping("CreationTime","CreationTime"),
                new SqlBulkCopyColumnMapping("MongoCreationTime","MongoCreationTime"),
                new SqlBulkCopyColumnMapping("ShiftDay","ShiftDetail_ShiftDay"),
                new SqlBulkCopyColumnMapping("MachineShiftItemName","ShiftDetail_MachineShiftName"),
                new SqlBulkCopyColumnMapping("StaffShiftItemName","ShiftDetail_StaffShiftName"),
                new SqlBulkCopyColumnMapping("ShiftSolutionName","ShiftDetail_SolutionName"),
                new SqlBulkCopyColumnMapping("DmpId","DmpId"),
                new SqlBulkCopyColumnMapping("PreviousLinkId","PreviousLinkId")
            };
        }

        private static DataTable BuildDataTable(string tableName, List<MongoState> source)
        {
            var dataTable = InitDataTable(tableName);

            foreach (var t in source)
            {
                var row = dataTable.NewRow();
                var startTime = t.CreationTime.MongoDateTimeParseExact();
                row["MachineId"] = Convert.ToInt64(t.MachineId);
                row["MachineCode"] = t.MachineCode;
                row["Code"] = t.Code;
                row["Name"] = string.Empty;
                row["StartTime"] = startTime;
                row["EndTime"] = DBNull.Value;
                row["Duration"] = 0;
                row["Memo"] = string.Empty;
                row["UserId"] = t.UserId;
                row["UserShiftDetailId"] = t.UserShiftDetailId;
                row["MachinesShiftDetailId"] = t.MachinesShiftDetailId;
                row["OrderId"] = t.OrderId;
                row["ProcessId"] = t.ProcessId;
                row["ProductId"] = t.ProductId;
                row["PartNo"] = t.PartNo;
                row["DateKey"] = Convert.ToInt32(startTime.ToString("yyyyMMdd"));
                row["ProgramName"] = t.ProgramName;
                row["IsShiftSplit"] = false;
                row["CreationTime"] = DateTime.Now;
                row["MongoCreationTime"] = t.CreationTime;
                row["ShiftDay"] = t.ShiftDay;
                row["MachineShiftItemName"] = t.MachineShiftItemName;
                row["StaffShiftItemName"] = t.StaffShiftItemName;
                row["ShiftSolutionName"] = t.ShiftSolutionName;
                row["DmpId"] = string.IsNullOrWhiteSpace(t.DmpId) ? Guid.Empty : new Guid(t.DmpId);
                row["PreviousLinkId"] = string.IsNullOrWhiteSpace(t.PreDmpId) ? Guid.Empty : new Guid(t.PreDmpId);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }


    

        private List<MongoState> GetMongoData(string lastSyncDateTime)
        {
            var mongoData = mongoStateManager.GetMongoStateWithNoSync(lastSyncDateTime).ToList();
            return mongoData;
        }
    }
}