using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Dapper;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Trace;
using Wimi.BtlCore.Trace.Dto;

namespace WIMI.BTL.EntityFramework.Repositories.Traceability
{
    public class TraceExportRepository : ITraceExportRepository
    {
        public async Task<IEnumerable<TraceExportItem>> ListTraceExportItem(TraceCatalogsInputDto input)
        {
            var whereSql = string.Empty;
            if (!input.PartNo.IsNullOrEmpty())
            {
                whereSql += $" AND tc.PartNo LIKE '%{input.PartNo.Trim()}%'";
            }

            if (input.DeviceGroupId.HasValue && input.DeviceGroupId.Value != 0)
            {
                whereSql += $" AND tc.DeviceGroupId = {input.DeviceGroupId.Value} ";
            }


            // 没有输入时，日期生效
            if (input.PartNo.IsNullOrWhiteSpace())
            {
                if (input.MachineId != null && input.MachineId.Count != 0)
                {
                    if (input.StartFirstTime.HasValue)
                    {
                        whereSql += $" AND tfr.EntryTime >= '{input.StartFirstTime.Value.ToLocalFormat()}' ";
                    }

                    if (input.StartLastTime.HasValue)
                    {
                        whereSql += $" AND tfr.EntryTime <= '{input.StartLastTime.Value.ToLocalFormat()}' ";
                    }
                    if (input.EndFirstTime.HasValue)
                    {
                        whereSql += $" AND tfr.LeftTime >= '{input.EndFirstTime.Value.ToLocalFormat()}' ";
                    }
                    if (input.EndLastTime.HasValue)
                    {
                        whereSql += $" AND tfr.LeftTime <= '{input.EndLastTime.Value.ToLocalFormat()}' ";
                    }
                }
                else
                {

                    if (input.StartFirstTime.HasValue)
                    {
                        whereSql += $" AND tc.OnlineTime >= '{input.StartFirstTime.Value.ToLocalFormat()}' ";
                    }

                    if (input.StartLastTime.HasValue)
                    {
                        whereSql += $" AND tc.OnlineTime <= '{input.StartLastTime.Value.ToLocalFormat()}' ";
                    }
                    if (input.EndFirstTime.HasValue)
                    {
                        whereSql += $" AND tc.OfflineTime >= '{input.EndFirstTime.Value.ToLocalFormat()}' ";
                    }
                    if (input.EndLastTime.HasValue)
                    {
                        whereSql += $" AND tc.OfflineTime <= '{input.EndLastTime.Value.ToLocalFormat()}' ";
                    }
                }

            }
            if (input.NgPartCatlogId != 0)
            {
                whereSql += $" AND tc.Id = {input.NgPartCatlogId} ";
            }

            if (input.MachineId != null && input.MachineId.Count != 0)
            {
                whereSql += $" AND tfr.MachineId IN ({input.MachineId.JoinAsString(",")}) ";
            }

            if (input.ShiftSolutionItemId != 0)
            {
                whereSql += $" AND msd.ShiftSolutionItemId = {input.ShiftSolutionItemId} ";
            }


            var sql = $@"SELECT DISTINCT tc.PartNo,
                                   CASE
                                       WHEN tc.OfflineTime IS NULL THEN
                                           '未下线'
                                       ELSE
                                           '已下线'
                                   END AS TraceStates,
                                   tc.Qualified,
                                   sst.Name AS ShiftItemName,
                                   m.Name AS MachineName,
                                   tfs.DisplayName AS FlowName,
                                   tfr.EntryTime,
                                   tfr.LeftTime,
                                   tfr.State
                            FROM dbo.TraceCatalogs AS tc
                                INNER JOIN dbo.TraceFlowRecords AS tfr  ON tfr.PartNo = tc.PartNo
                                INNER JOIN dbo.MachinesShiftDetails AS msd ON msd.Id = tc.MachineShiftDetailId
                                INNER JOIN dbo.ShiftSolutionItems AS sst ON sst.Id = msd.ShiftSolutionItemId
                                INNER JOIN dbo.Machines AS m ON tfr.MachineId = m.Id
                                INNER JOIN dbo.TraceFlowSettings AS tfs ON tfs.Code = tfr.FlowCode And tfs.DeviceGroupId = tc.DeviceGroupId
                            WHERE m.IsDeleted = 0 {whereSql} " + Environment.NewLine;
            for (int i = 0; i < input.UnionTables.Item1.Count; i++)
            {
                //查询的数据涉及到分表
                sql += $@"
UNION ALL
SELECT tc.PartNo,
                                   CASE
                                       WHEN tc.OfflineTime IS NULL THEN
                                           '未下线'
                                       ELSE
                                           '已下线'
                                   END AS TraceStates,
                                   tc.Qualified,
                                   sst.Name AS ShiftItemName,
                                   m.Name AS MachineName,
                                   tfs.DisplayName AS FlowName,
                                   tfr.EntryTime,
                                   tfr.LeftTime,
                                   tfr.State
                            FROM dbo.[{input.UnionTables.Item1[i]}] AS tc
                                INNER JOIN dbo.[{input.UnionTables.Item2[i]}] AS tfr  ON tfr.PartNo = tc.PartNo
                                INNER JOIN dbo.MachinesShiftDetails AS msd ON msd.Id = tc.MachineShiftDetailId
                                INNER JOIN dbo.ShiftSolutionItems AS sst ON sst.Id = msd.ShiftSolutionItemId
                                INNER JOIN dbo.Machines AS m ON tfr.MachineId = m.Id
                                INNER JOIN dbo.TraceFlowSettings AS tfs ON tfs.Code = tfr.FlowCode
                            WHERE m.IsDeleted = 0 {whereSql}" + Environment.NewLine;
            }

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                return await conn.QueryAsync<TraceExportItem>(sql);
            }
        }
    }
}