using Abp.Domain.Repositories;
using Abp.Logging;

namespace Wimi.BtlCore.BackgroundJobs.Process
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Wimi.BtlCore.BasicData.Capacities;
    using Wimi.BtlCore.BasicData.Capacities.Mongo;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Extensions;

    public class CapacityProcess : BasicProcess
    {
        private readonly ICapacityRepository capacityRepository;
        private readonly MongoCapacityManager mongoCapacityManager;

        public CapacityProcess(IRepository<SyncDataFlag> syncDataFlagRepository, ICapacityRepository capacityRepository,
             MongoCapacityManager mongoCapacityManager) : base(syncDataFlagRepository)
        {
            this.capacityRepository = capacityRepository;
            this.mongoCapacityManager = mongoCapacityManager;
        }

        /// <summary>
        /// 处理流程
        /// </summary>
        public bool Process(string mongoTableName, string targetTable)
        {
            try
            {
                // 1.获取最新更新时间
                var lastSyncDateTime = this.capacityRepository.GetMaxCapacitySyncDateTime();
                this.Logger.Info($"[{this.GetType().Name}], 获取最新同步时间:【{lastSyncDateTime}】");

                // 2.根据取得的时间段从Mongo中取得数据
                var source = this.GetMongoData(lastSyncDateTime);

                this.Logger.Info($"[{this.GetType().Name}], 从Mongo获取到数据:【{source.Count}】 条");

                if (source.Count == 0)
                {
                    return false;
                }
               
                // 3.创建用于暂存数据的DataTable
                var dataTable = BuildDataTable(mongoTableName,source);

                // 5.把数据搬运到SQL Server中
                var syncTime = source.Last().CreationTime;
                var columnMappings = GetSqlBulkCopyColumnMappings();
                this.InsertIntoSqlServer(dataTable, columnMappings, targetTable, syncTime);
                this.Logger.Info($"[{this.GetType().Name}], 更新最后索引:【{syncTime}】 ");
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal($"[Job] Capacity Sync Error,Message:{ex}");
                return false;
            }

            return true;
        }


        private static DataTable InitDataTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("MachineId", typeof(long));
            dt.Columns.Add("MachineCode", typeof(string));
            dt.Columns.Add("Yield", typeof(decimal));
            dt.Columns.Add("AccumulateCount", typeof(decimal));
            dt.Columns.Add("OriginalCount", typeof(decimal));
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("StartTime", typeof(DateTime));
            dt.Columns.Add("EndTime", typeof(DateTime));
            dt.Columns.Add("Duration", typeof(decimal));
            dt.Columns.Add("ProgramName", typeof(string));
            dt.Columns.Add("IsValid", typeof(bool));
            dt.Columns.Add("Memo", typeof(string));
            dt.Columns.Add("UserId", typeof(int));
            dt.Columns.Add("UserShiftDetailId", typeof(int));
            dt.Columns.Add("MachinesShiftDetailId", typeof(int));
            dt.Columns.Add("OrderId", typeof(int));
            dt.Columns.Add("ProcessId", typeof(int));
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("PartNo", typeof(string));
            dt.Columns.Add("DateKey", typeof(int));
            dt.Columns.Add("PlanId", typeof(int));
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
                new SqlBulkCopyColumnMapping("Yield", "Yield"),
                new SqlBulkCopyColumnMapping("AccumulateCount", "AccumulateCount"),
                new SqlBulkCopyColumnMapping("OriginalCount", "OriginalCount"),
                new SqlBulkCopyColumnMapping("Rate", "Rate"),
                new SqlBulkCopyColumnMapping("StartTime", "StartTime"),
                new SqlBulkCopyColumnMapping("EndTime", "EndTime"),
                new SqlBulkCopyColumnMapping("Duration", "Duration"),
                new SqlBulkCopyColumnMapping("ProgramName", "ProgramName"),
                new SqlBulkCopyColumnMapping("IsValid", "IsValid"),
                new SqlBulkCopyColumnMapping("Memo", "Memo"),
                new SqlBulkCopyColumnMapping("UserId", "UserId"),
                new SqlBulkCopyColumnMapping("UserShiftDetailId","UserShiftDetailId"),
                new SqlBulkCopyColumnMapping("MachinesShiftDetailId", "MachinesShiftDetailId"),
                new SqlBulkCopyColumnMapping("OrderId", "OrderId"),
                new SqlBulkCopyColumnMapping("ProcessId", "ProcessId"),
                new SqlBulkCopyColumnMapping("ProductId", "ProductId"),
                new SqlBulkCopyColumnMapping("PartNo", "PartNo"),
                new SqlBulkCopyColumnMapping("DateKey", "DateKey"),
                new SqlBulkCopyColumnMapping("PlanId", "PlanId"),
                new SqlBulkCopyColumnMapping("MongoCreationTime","MongoCreationTime"),
                new SqlBulkCopyColumnMapping("ShiftDay","ShiftDetail_ShiftDay"),
                new SqlBulkCopyColumnMapping("MachineShiftItemName","ShiftDetail_MachineShiftName"),
                new SqlBulkCopyColumnMapping("StaffShiftItemName","ShiftDetail_StaffShiftName"),
                new SqlBulkCopyColumnMapping("ShiftSolutionName","ShiftDetail_SolutionName"),
                new SqlBulkCopyColumnMapping("DmpId","DmpId"),
                new SqlBulkCopyColumnMapping("PreviousLinkId","PreviousLinkId")
            };
        }

        private static DataTable BuildDataTable(string tableName, List<MongoCapacity> source)
        {
            var dataTable = InitDataTable(tableName);

            for (var i = 0; i < source.Count(); i++)
            {
                var row = dataTable.NewRow();
                var startTime = source[i].CreationTime.MongoDateTimeParseExact();
                row["MachineId"] = Convert.ToInt64(source[i].MachineId);
                row["MachineCode"] = source[i].MachineCode;
                row["Yield"] = source[i].Yield;
                row["AccumulateCount"] = source[i].AccumulateCount;
                row["OriginalCount"] = source[i].OriginalCount;
                row["Rate"] = 1;
                row["StartTime"] = startTime;
                row["EndTime"] = DBNull.Value;
                row["Duration"] = 0;
                row["ProgramName"] = source[i].ProgramName;
                row["IsValid"] = true;
                row["Memo"] = string.Empty;
                row["UserId"] = source[i].UserId;
                row["UserShiftDetailId"] = source[i].UserShiftDetailId;
                row["MachinesShiftDetailId"] = source[i].MachinesShiftDetailId;
                row["OrderId"] = source[i].OrderId;
                row["ProcessId"] = source[i].ProcessId;
                row["ProductId"] = source[i].ProductId;
                row["PartNo"] = source[i].PartNo;
                row["DateKey"] = Convert.ToInt32(startTime.ToString("yyyyMMdd"));
                row["PlanId"] = source[i].PlanId;
                row["MongoCreationTime"] = source[i].CreationTime;
                row["ShiftDay"] = source[i].ShiftDay;
                row["MachineShiftItemName"] = source[i].MachineShiftItemName;
                row["StaffShiftItemName"] = source[i].StaffShiftItemName;
                row["ShiftSolutionName"] = source[i].ShiftSolutionName;
                row["DmpId"] = string.IsNullOrWhiteSpace(source[i].DmpId) ? Guid.Empty : new Guid(source[i].DmpId);
                row["PreviousLinkId"] = string.IsNullOrWhiteSpace(source[i].PreDmpId) ? Guid.Empty : new Guid(source[i].PreDmpId);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private List<MongoCapacity> GetMongoData(string lastSyncDateTime)
        {
            var mongoData = mongoCapacityManager.GetMongoCapacityWithNoSync(lastSyncDateTime).ToList();
            return mongoData;
        }

    }
}