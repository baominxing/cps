using Abp.Domain.Repositories;

namespace Wimi.BtlCore.BackgroundJobs.Process
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Abp.Logging;
    using Wimi.BtlCore.BasicData.Alarms.Mongo;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Extensions;

    public class AlarmProcess : BasicProcess
    {
        private readonly IAlarmRepository alarmRepository;
        private readonly MongoAlarmManager mongoAlarmManager;

        public AlarmProcess(IRepository<SyncDataFlag> syncDataFlagRepository, 
            IAlarmRepository alarmRepository,
            MongoAlarmManager mongoAlarmManager) : base(syncDataFlagRepository)
        {
            this.alarmRepository = alarmRepository;
            this.mongoAlarmManager = mongoAlarmManager;
        }

        /// <summary>
        /// 处理流程    
        /// </summary>
        public bool Process(string mongoTable, string targetTable)
        {
            try
            {
                // 1.获取最新更新时间
                var lastSyncDateTime = this.alarmRepository.GetMaxSyncAlarmDateTime();
                this.Logger.Info($"[{this.GetType().Name}], 获取最新同步时间:【{lastSyncDateTime}】");

                // 2.根据取得的时间段从Mongo中取得数据
                var source = GetMongoData(lastSyncDateTime);

                this.Logger.Info($"[{this.GetType().Name}], 从Mongo获取到数据:【{source.Count}】 条");

                if (!source.Any())
                {
                    return false;
                }

                // 3.创建用于暂存数据的DataTable
                var dataTable = BuildDataTable(mongoTable, source);

                // 4.把数据搬运到SQL Server中
                var columnMappings = GetSqlBulkCopyColumnMappings();
                var syncTime = source.Last().CreationTime;
                this.InsertIntoSqlServer(dataTable, columnMappings, targetTable, syncTime);
                this.Logger.Info($"[{this.GetType().Name}], 更新最后索引:【{syncTime}】 ");
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal($"[Job] Alarm Sync Error,Message:{ex}");
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
            dt.Columns.Add("Message", typeof(string));
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
            dt.Columns.Add("ShiftDay", typeof(string));
            dt.Columns.Add("MachineShiftItemName", typeof(string));
            dt.Columns.Add("StaffShiftItemName", typeof(string));
            dt.Columns.Add("ShiftSolutionName", typeof(string));
            dt.Columns.Add("MongoCreationTime", typeof(string));
            return dt;
        }

        private static IEnumerable<SqlBulkCopyColumnMapping> GetSqlBulkCopyColumnMappings()
        {
            return new List<SqlBulkCopyColumnMapping>
            {
                new SqlBulkCopyColumnMapping("MachineId", "MachineId"),
                new SqlBulkCopyColumnMapping("MachineCode", "MachineCode"),
                new SqlBulkCopyColumnMapping("Code", "Code"),
                new SqlBulkCopyColumnMapping("Message", "Message"),
                new SqlBulkCopyColumnMapping("StartTime", "StartTime"),
                new SqlBulkCopyColumnMapping("EndTime", "EndTime"),
                new SqlBulkCopyColumnMapping("Duration", "Duration"),
                new SqlBulkCopyColumnMapping("Memo", "Memo"),
                new SqlBulkCopyColumnMapping("UserId", "UserId"),
                new SqlBulkCopyColumnMapping("UserShiftDetailId", "UserShiftDetailId"),
                new SqlBulkCopyColumnMapping("MachinesShiftDetailId", "MachinesShiftDetailId"),
                new SqlBulkCopyColumnMapping("OrderId", "OrderId"),
                new SqlBulkCopyColumnMapping("ProcessId", "ProcessId"),
                new SqlBulkCopyColumnMapping("ProductId", "ProductId"),
                new SqlBulkCopyColumnMapping("PartNo", "PartNo"),
                new SqlBulkCopyColumnMapping("DateKey", "DateKey"),
                new SqlBulkCopyColumnMapping("ProgramName", "ProgramName"),
                new SqlBulkCopyColumnMapping("MongoCreationTime","MongoCreationTime"),


                new SqlBulkCopyColumnMapping("ShiftDay","ShiftDetail_ShiftDay"),
                new SqlBulkCopyColumnMapping("MachineShiftItemName","ShiftDetail_MachineShiftName"),
                new SqlBulkCopyColumnMapping("StaffShiftItemName","ShiftDetail_StaffShiftName"),
                new SqlBulkCopyColumnMapping("ShiftSolutionName","ShiftDetail_SolutionName")
        };
        }

        private static DataTable BuildDataTable(string tableName, List<MongoAlarm> source)
        {
            var dataTable = InitDataTable(tableName);
            for (var i = 0; i < source.Count; i++)
            {
                var row = dataTable.NewRow();

                var startTime = source[i].CreationTime.MongoDateTimeParseExact();
                row["MachineId"] = Convert.ToInt64(source[i].MachineId.ToString());
                row["MachineCode"] = source[i].MachineCode;
                row["Code"] = source[i].Code;
                row["Message"] = source[i].Message;
                row["StartTime"] = startTime;
                row["EndTime"] = DBNull.Value;
                row["Duration"] = 0;
                row["Memo"] = string.Empty;
                row["UserId"] = source[i].UserId;
                row["UserShiftDetailId"] = source[i].UserShiftDetailId;
                row["MachinesShiftDetailId"] = source[i].MachinesShiftDetailId;
                row["OrderId"] = source[i].OrderId;
                row["ProcessId"] = source[i].ProcessId;
                row["ProductId"] = source[i].ProductId;
                row["PartNo"] = source[i].PartNo;
                row["DateKey"] = Convert.ToInt32(startTime.ToString("yyyyMMdd"));
                row["ProgramName"] = source[i].ProgramName;
                row["MongoCreationTime"] = source[i].CreationTime;
                row["ShiftDay"] = source[i].ShiftDay;
                row["MachineShiftItemName"] = source[i].MachineShiftItemName;
                row["StaffShiftItemName"] = source[i].StaffShiftItemName;
                row["ShiftSolutionName"] = source[i].ShiftSolutionName;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private List<MongoAlarm> GetMongoData(string lastSyncDateTime)
        {
            var mongoData = mongoAlarmManager.GetMongoAlarmWithNoSync(lastSyncDateTime).ToList();
            return mongoData;
        }
    }
}