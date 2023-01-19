using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wimi.BtlCore.Migrations
{
    /// <inheritdoc />
    public partial class AddComments20230110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                comment: "工单key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "WorkOrderTasks",
                type: "bigint",
                nullable: false,
                comment: "用户key",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "WorkOrderTasks",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                comment: "合格数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OutputCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                comment: "输出数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                comment: "设备key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "WorkOrderTasks",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                comment: "不合格数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "合格数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PutVolume",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "设置值",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductionPlanId",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "生产计划key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "工序key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OutputCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "输出数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLastProcessOrder",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                comment: "是否最后工序",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "不合格数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "CompletionRate",
                table: "WorkOrders",
                type: "decimal(18,2)",
                nullable: false,
                comment: "完成率",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "AimVolume",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                comment: "目标值",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderTaskId",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "工单任务key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonsId",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "不良原因key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "WeChatNotifications",
                type: "bigint",
                nullable: false,
                comment: "用户key",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationTypeId",
                table: "WeChatNotifications",
                type: "int",
                nullable: false,
                comment: "通知消息类型Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "WeChatNotifications",
                type: "bit",
                nullable: false,
                comment: "是否启用（1启用,0未启用）",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingStationDisplayName",
                table: "TraceRelatedMachines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "工位名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WorkingStationCode",
                table: "TraceRelatedMachines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "工位编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TraceFlowSettingId",
                table: "TraceRelatedMachines",
                type: "int",
                nullable: false,
                comment: "流程设定ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "TraceRelatedMachines",
                type: "int",
                nullable: false,
                comment: "设备ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "TraceRelatedMachines",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "WriteIntoPlcViaFlowData",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                comment: "数据触发时写信号",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "WriteIntoPlcViaFlow",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                comment: "流程触发时写信号",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "TriggerEndFlowStyle",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "结束流程方式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StationType",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "工位类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SourceOfPartNo",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "工件来源",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QualityMakerFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                comment: "质量责任归属到特定流程",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                comment: "上一流程ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NextFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                comment: "下一流程ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "NeedHandlerRelateData",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                comment: "是否处理相关数据",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "FlowType",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "流程类别",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FlowSeq",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "流程顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ExtensionData",
                table: "TraceFlowSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "数据",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "TraceFlowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "流程名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                comment: "设备组ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TraceFlowSettings",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "流程编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "TraceFlowRecords",
                type: "bigint",
                nullable: false,
                comment: "用户ID",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "TraceFlowSettingId",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                comment: "流程设定ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Tag",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                comment: "流程标识",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Station",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "工位",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                comment: "流程状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "TraceFlowRecords",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "工件编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                comment: "设备ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeftTime",
                table: "TraceFlowRecords",
                type: "datetime2",
                nullable: true,
                comment: "离开时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlowDisplayName",
                table: "TraceFlowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "流程名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlowCode",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "流程编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtensionData",
                table: "TraceFlowRecords",
                type: "nvarchar(max)",
                nullable: true,
                comment: "数据",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EntryTime",
                table: "TraceFlowRecords",
                type: "datetime2",
                nullable: false,
                comment: "进入时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "TraceFlowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档表",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Qualified",
                table: "TraceCatalogs",
                type: "bit",
                nullable: true,
                comment: "是否合格",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                comment: "计划Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "TraceCatalogs",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "工件编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineTime",
                table: "TraceCatalogs",
                type: "datetime2",
                nullable: false,
                comment: "上线时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfflineTime",
                table: "TraceCatalogs",
                type: "datetime2",
                nullable: true,
                comment: "下线时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                comment: "设备班次详情Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReworkPart",
                table: "TraceCatalogs",
                type: "bit",
                nullable: true,
                comment: "是否返工",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                comment: "设备组ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "TraceCatalogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档表",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramF",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号F",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramE",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号E",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramD",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号D",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramC",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号C",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramB",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号B",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramA",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "程序号A",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Tongs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tongs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tongs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Tongs",
                type: "int",
                nullable: false,
                comment: "容量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ThirdpartyApis",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "地址",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "ThirdpartyApis",
                type: "int",
                nullable: false,
                comment: "类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ThirdpartyApis",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "名字",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ThirdpartyApis",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionEndDateUtc",
                table: "Tenants",
                type: "datetime2",
                nullable: true,
                comment: "订阅结束日期Utc",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LogoId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Logo Id",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoFileType",
                table: "Tenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "Logo文件类型",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsInTrialPeriod",
                table: "Tenants",
                type: "bit",
                nullable: false,
                comment: "是否正在试用期间",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomCssId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: true,
                comment: "自定义样式Id",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalCount",
                table: "SyncDataFlags",
                type: "int",
                nullable: false,
                comment: "总共同步数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessName",
                table: "SyncDataFlags",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "作业名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "LastSyncTime",
                table: "SyncDataFlags",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "上次同步时间",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "LastSyncCount",
                table: "SyncDataFlags",
                type: "int",
                nullable: false,
                comment: "上次同步数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "States",
                type: "int",
                nullable: true,
                comment: "用户班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "States",
                type: "int",
                nullable: true,
                comment: "用户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "States",
                type: "datetime2",
                nullable: true,
                comment: "班次开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                comment: "人员班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                comment: "班次方案名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "States",
                type: "datetime2",
                nullable: true,
                comment: "班次日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "States",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "程序号",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "States",
                type: "int",
                nullable: false,
                comment: "产品Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "States",
                type: "int",
                nullable: true,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousLinkId",
                table: "States",
                type: "uniqueidentifier",
                nullable: true,
                comment: "连接前面一笔记录的DmpId",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "零件码",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "States",
                type: "int",
                nullable: true,
                comment: "工单Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "状态名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "创建时间 记录在MongoDb",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "States",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "States",
                type: "int",
                nullable: true,
                comment: "设备班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "States",
                type: "bigint",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "States",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShiftSplit",
                table: "States",
                type: "bit",
                nullable: false,
                comment: "是否切班产生",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "States",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "States",
                type: "decimal(18,2)",
                nullable: false,
                comment: "持续时长（秒）",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpId",
                table: "States",
                type: "uniqueidentifier",
                nullable: false,
                comment: "唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "States",
                type: "int",
                nullable: true,
                comment: "系统日历 20230106",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "状态编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "StateInfos",
                type: "int",
                nullable: false,
                comment: "状态类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalCode",
                table: "StateInfos",
                type: "int",
                nullable: true,
                comment: "原始采集编号",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsStatic",
                table: "StateInfos",
                type: "bit",
                nullable: false,
                comment: "静态状态无法编辑和删除，由程序创建",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaned",
                table: "StateInfos",
                type: "bit",
                nullable: false,
                comment: "是否计划内",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Hexcode",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "状态颜色",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "状态名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "状态编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardCostTime",
                table: "StandardTime",
                type: "decimal(18,2)",
                nullable: false,
                comment: "用时",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "StandardTime",
                type: "int",
                nullable: false,
                comment: "产品Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "StandardTime",
                type: "int",
                nullable: false,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "StandardTime",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CycleRate",
                table: "StandardTime",
                type: "int",
                nullable: false,
                comment: "倍率",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TargetYiled",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                comment: "目标产量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                comment: "班次key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftTargetYileds",
                type: "datetime2",
                nullable: false,
                comment: "班次日",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                comment: "产品key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShiftSolutions",
                type: "nvarchar(max)",
                nullable: true,
                comment: "班次方案名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ShiftSolutionItems",
                type: "datetime2",
                nullable: false,
                comment: "班次开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftSolutionItems",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShiftSolutionItems",
                type: "nvarchar(max)",
                nullable: true,
                comment: "班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsNextDay",
                table: "ShiftSolutionItems",
                type: "bit",
                nullable: false,
                comment: "班次是否跨天",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ShiftSolutionItems",
                type: "datetime2",
                nullable: false,
                comment: "班次结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "ShiftSolutionItems",
                type: "decimal(18,2)",
                nullable: false,
                comment: "班次持续时间 秒",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                comment: "具体班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftHistories",
                type: "datetime2",
                nullable: false,
                comment: "班次日",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                comment: "班次日期",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                comment: "班次开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftYearName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "班次年名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftYear",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "班次年",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftWeekName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "班次周名称 2020-03 周",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftWeek",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "班次周",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftMonthName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "班次月名称 2020-01 月",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftMonth",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "班次月",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftItemSeq",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "具体班次顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftItemId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "具体班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDayName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "班次日名称 2020-02-08 日",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                comment: "班次日",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "设备班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                comment: "班次结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "ShiftCalendars",
                type: "bigint",
                nullable: false,
                comment: "班次时长，秒",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                comment: "维修开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestUserId",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                comment: "申请人",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RequestMemo",
                table: "RepairRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "请求备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "RepairRequests",
                type: "datetime2",
                nullable: false,
                comment: "申请日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "RepairUserId",
                table: "RepairRequests",
                type: "int",
                nullable: true,
                comment: "维修人",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RepairMemo",
                table: "RepairRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "维修备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RepairDate",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                comment: "维修日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShutdown",
                table: "RepairRequests",
                type: "bit",
                nullable: false,
                comment: "是否关机",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                comment: "维修结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "RepairRequests",
                type: "decimal(18,2)",
                nullable: false,
                comment: "耗费时间",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RepairRequests",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: false,
                comment: "状态Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "ReasonFeedbackRecords",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "状态编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ReasonFeedbackRecords",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EndUserId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: true,
                comment: "结束用户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ReasonFeedbackRecords",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "ReasonFeedbackRecords",
                type: "decimal(18,2)",
                nullable: false,
                comment: "持续时间",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Spec",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "规格",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupId",
                table: "Products",
                type: "int",
                nullable: false,
                comment: "产品组key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PartType",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "零件类型",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IsHalfFinished",
                table: "Products",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "是否半成品",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DrawingNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "图纸号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "ProductionPlans",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "单位",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "合格数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PutVolume",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "设置值",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductionPlanState",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "生产计划状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "产品key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "工序编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "ProductionPlans",
                type: "nvarchar(max)",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: false,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "不良数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "工艺key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ClientName",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "客户端名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AimVolume",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                comment: "目标值",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualStartDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: true,
                comment: "实际开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualEndDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: true,
                comment: "实际结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "名字",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "ProductGroups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "YieldSummaryType",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "产量计算方式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "YieldCounterMachineId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "产量计数器设备",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TargetType",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "目标量维度",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TargetAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "目标量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "计划状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RealStartTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                comment: "实际开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RealEndTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                comment: "实际截止时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "产品名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "产品ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "已完成量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlanStartTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                comment: "计划开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlanName",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "计划名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlanEndTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                comment: "计划截止时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlanCode",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "计划编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "计划产量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PauseTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                comment: "暂停时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTimeRangeSelect",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                comment: "时间范围是否选中",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoStartNextPlan",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                comment: "自动开启后续计划",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoFinishCurrentPlan",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                comment: "计划生产量达标后自动关闭计划",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                comment: "设备组ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Process",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Process",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Process",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "SolutionId",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                comment: "班次方案ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftTargetAmount",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                comment: "班次目标量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftName",
                table: "PlanTargets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                comment: "班次ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "PerformancePersonnelOnDevices",
                type: "bigint",
                nullable: false,
                comment: "上线时间",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineDate",
                table: "PerformancePersonnelOnDevices",
                type: "datetime2",
                nullable: false,
                comment: "设备key",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "PerformancePersonnelOnDevices",
                type: "int",
                nullable: false,
                comment: "用户Key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "PartDefects",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "零件编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                comment: "不良原因ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DefectivePartId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                comment: "不良部位ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveMachineId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                comment: "不良设备ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "OnlineAndOfflineLogs",
                type: "bigint",
                nullable: false,
                comment: "用户Key",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineDateTime",
                table: "OnlineAndOfflineLogs",
                type: "datetime2",
                nullable: false,
                comment: "上线时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfflineDateTime",
                table: "OnlineAndOfflineLogs",
                type: "datetime2",
                nullable: true,
                comment: "下线时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "OnlineAndOfflineLogs",
                type: "int",
                nullable: false,
                comment: "设备key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "NotificationTypes",
                type: "int",
                nullable: true,
                comment: "父节点Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Code，参照部门实现方式",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "NotificationTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "用户key",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "TriggerType",
                table: "NotificationRules",
                type: "int",
                nullable: false,
                comment: "触发类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationRules",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "规则名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MessageType",
                table: "NotificationRules",
                type: "int",
                nullable: false,
                comment: "消息类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceGroupIds",
                table: "NotificationRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "设备组Ids",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TriggerCondition",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                comment: "触发条件",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                comment: "班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationRuleId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                comment: "通知规则Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NoticeUserIds",
                table: "NotificationRuleDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "通知人员Id(1,2,3)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "NotificationRuleDetails",
                type: "bit",
                nullable: false,
                comment: "是否启用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                comment: "消息状态(0:未发送,1:已发送)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationType",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                comment: "通知方式（微信，邮件）",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "NoticedUserId",
                table: "NotificationRecords",
                type: "bigint",
                nullable: false,
                comment: "通知人Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "MessageType",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                comment: "消息类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "NotificationRecords",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "消息内容",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RootDeviceGroupCode",
                table: "Notices",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "车间代码",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Notices",
                type: "bit",
                nullable: false,
                comment: "是否启用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "公告内容",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "MaintainPlans",
                type: "datetime2",
                nullable: false,
                comment: "计划生效日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "PersonInChargeId",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                comment: "负责人",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintainPlans",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "MaintainPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                comment: "设备编号",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IntervalDate",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                comment: "间隔日期",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "MaintainPlans",
                type: "datetime2",
                nullable: false,
                comment: "计划失效日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MaintainPlans",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: false,
                comment: "计划保养日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "MaintainOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaintainUserId",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                comment: "保养用户Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MaintainPlanCode",
                table: "MaintainOrders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "保养计划编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MaintainDate",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                comment: "实际保养日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "MaintainOrders",
                type: "decimal(18,2)",
                nullable: false,
                comment: "耗费时间",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MaintainOrders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ValueFactor",
                table: "MachineVariables",
                type: "float",
                nullable: false,
                comment: "值类型",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                comment: "类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Methodparam",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "方法参数",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Index",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                comment: "顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineVariables",
                type: "uniqueidentifier",
                nullable: false,
                comment: "设备唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceAddress",
                table: "MachineVariables",
                type: "nvarchar(max)",
                nullable: false,
                comment: "设备地址",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultValue",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "默认值",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "数据类型",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "DataLength",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                comment: "数据长度",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "Access",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                comment: "存取",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineTypes",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备类型名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "MachineTypes",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备类型描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                comment: "具体班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "MachinesShiftDetails",
                type: "datetime2",
                nullable: false,
                comment: "班次日",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "MachineShiftEffectiveIntervals",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachineShiftEffectiveIntervals",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineShiftEffectiveIntervals",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MachineShiftEffectiveIntervals",
                type: "datetime2",
                nullable: false,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                comment: "操作类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                comment: "班次方案Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                comment: "显示顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UId",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "硬件用户名",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SortSeq",
                table: "Machines",
                type: "int",
                nullable: false,
                comment: "显示顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProductKey",
                table: "Machines",
                type: "nvarchar(max)",
                nullable: true,
                comment: "弃用",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "硬件密码",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "设备名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineTypeId",
                table: "Machines",
                type: "int",
                nullable: false,
                comment: "设备类型Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCalLineCapacity",
                table: "Machines",
                type: "bit",
                nullable: false,
                comment: "弃用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Machines",
                type: "bit",
                nullable: false,
                comment: "是否启用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                table: "Machines",
                type: "uniqueidentifier",
                nullable: true,
                comment: "图片Id(GUID)，在表AppBinaryObjects中",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "Machines",
                type: "uniqueidentifier",
                nullable: false,
                comment: "唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceSecret",
                table: "Machines",
                type: "nvarchar(max)",
                nullable: true,
                comment: "弃用",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "弃用",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Machines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "设备描述",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Machines",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramNo",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "程序号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProcessCode",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "工序编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachinePrograms",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                comment: "产品key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                comment: "工序key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                comment: "设备key",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MachineProcesses",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangeProductUserId",
                table: "MachineProcesses",
                type: "int",
                nullable: true,
                comment: "更改产品的用户key",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TcpPort",
                table: "MachineNetConfigs",
                type: "int",
                nullable: true,
                comment: "端口",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachineNetConfigs",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "MachineNetConfigs",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                comment: "IP地址",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineNetConfigs",
                type: "uniqueidentifier",
                nullable: false,
                comment: "设备唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "参数单位",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SortSeq",
                table: "MachineGatherParams",
                type: "int",
                nullable: false,
                comment: "显示顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "参数名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Min",
                table: "MachineGatherParams",
                type: "float",
                nullable: false,
                comment: "参数最小值",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Max",
                table: "MachineGatherParams",
                type: "float",
                nullable: false,
                comment: "参数最大值",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<Guid>(
                name: "MachineVariableId",
                table: "MachineGatherParams",
                type: "uniqueidentifier",
                nullable: true,
                comment: "唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "MachineGatherParams",
                type: "bigint",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachineGatherParams",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForVisual",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否在看板显示",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForStatus",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否在实时状态页显示",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForParam",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否在运行参数页显示",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Hexcode",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "参数显示颜色",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayStyle",
                table: "MachineGatherParams",
                type: "int",
                nullable: false,
                comment: "参数显示方式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "MachineGatherParams",
                type: "nvarchar(max)",
                nullable: true,
                comment: "参数数据类型",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "参数编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "MachineDrivers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "类型名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDrivers",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "Enable",
                table: "MachineDrivers",
                type: "bit",
                nullable: false,
                comment: "是否启动",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Driver",
                table: "MachineDrivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "驱动",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineDrivers",
                type: "uniqueidentifier",
                nullable: false,
                comment: "设备唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDeviceGroups",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "MachineDeviceGroups",
                type: "int",
                nullable: false,
                comment: "设备组Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceGroupCode",
                table: "MachineDeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "设备组编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "班次信息ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "MachineDefectiveRecords",
                type: "datetime2",
                nullable: false,
                comment: "日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "产品ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "设备ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonsId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "不良原因ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                comment: "数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TenantTaxNo",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租客号码",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantLegalName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                comment: "法定名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantAddress",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户地址",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                comment: "发票号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                comment: "发票日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "InfoLogs",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ProcName",
                table: "InfoLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "执行存储过程名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "InfoLogs",
                type: "datetime2",
                nullable: false,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "InfoLogs",
                type: "int",
                nullable: true,
                comment: "执行时长，秒",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InfoLogs",
                type: "uniqueidentifier",
                nullable: false,
                comment: "GUID",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StratTime",
                table: "InfoLogDetails",
                type: "datetime2",
                nullable: false,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Step",
                table: "InfoLogDetails",
                type: "nvarchar(max)",
                nullable: true,
                comment: "步骤",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "InfoLogDetails",
                type: "nvarchar(max)",
                nullable: true,
                comment: "信息",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InfoLogId",
                table: "InfoLogDetails",
                type: "uniqueidentifier",
                nullable: false,
                comment: "InfoLogs",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "InfoLogDetails",
                type: "datetime2",
                nullable: false,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "InfoLogDetails",
                type: "int",
                nullable: false,
                comment: "执行时长，秒",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AffectedRowCount",
                table: "InfoLogDetails",
                type: "int",
                nullable: true,
                comment: "影响的行数",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Friendships",
                type: "bigint",
                nullable: false,
                comment: "用户Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Friendships",
                type: "int",
                nullable: true,
                comment: "租户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Friendships",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FriendUserName",
                table: "Friendships",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "关联用户名",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<long>(
                name: "FriendUserId",
                table: "Friendships",
                type: "bigint",
                nullable: false,
                comment: "关联用户Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "FriendTenantId",
                table: "Friendships",
                type: "int",
                nullable: true,
                comment: "关联租户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FriendTenancyName",
                table: "Friendships",
                type: "nvarchar(max)",
                nullable: true,
                comment: "关联租户名",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FriendProfilePictureId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: true,
                comment: "关联用户图片标识",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "FmsCutterSettings",
                type: "int",
                nullable: false,
                comment: "字段类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "FmsCutterSettings",
                type: "int",
                nullable: false,
                comment: "顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow",
                table: "FmsCutterSettings",
                type: "bit",
                nullable: false,
                comment: "是否显示",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "FmsCutterSettings",
                type: "nvarchar(max)",
                nullable: true,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WarningLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "预警寿命",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "UseType",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                comment: "使用类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                comment: "刀具类型",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                comment: "刀具状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "预设寿命",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "FmsCutters",
                type: "int",
                nullable: true,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LengthCompensate",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "长度补偿",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Length",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "长度",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiameterCompensate",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "直径补偿",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Diameter",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "直径",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CutterStockNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                comment: "刀库编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                comment: "刀具编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CutterCase",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                comment: "刀套编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                comment: "当前寿命",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CountType",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                comment: "计数类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CompensateNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                comment: "补偿号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FmsCutterId",
                table: "FmsCutterExtends",
                type: "int",
                nullable: false,
                comment: "刀具Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FieldValue",
                table: "FmsCutterExtends",
                type: "nvarchar(max)",
                nullable: true,
                comment: "字段值",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomFieldId",
                table: "FmsCutterExtends",
                type: "int",
                nullable: false,
                comment: "自定义字段Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "FlexibleCrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "版本--产品下的版本号唯一",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "FlexibleCrafts",
                type: "int",
                nullable: false,
                comment: "产品Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FlexibleCrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TongId",
                table: "FlexibleCraftProcesses",
                type: "int",
                nullable: false,
                comment: "夹具Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Sequence",
                table: "FlexibleCraftProcesses",
                type: "int",
                nullable: false,
                comment: "顺序号",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FlexibleCraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CraftProcesseId",
                table: "FlexibleCraftProcesseMaps",
                type: "int",
                nullable: false,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "FlexibleCraftProcesseMaps",
                type: "int",
                nullable: false,
                comment: "工艺Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureNumber",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "nvarchar(max)",
                nullable: true,
                comment: "程序号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CutterId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                comment: "刀具Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CraftProcesseId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                comment: "工艺Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                comment: "状态编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "FeedbackCalendars",
                type: "int",
                nullable: false,
                comment: "持续时间 分钟",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Cron",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                comment: "表达式",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "FeedbackCalendarDetails",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FeedbackCalendarId",
                table: "FeedbackCalendarDetails",
                type: "int",
                nullable: false,
                comment: "反馈频率Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                comment: "返回引用它的 CATCH 块范围特有的错误状态",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Serverity",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                comment: "返回特定于引用它的 CATCH 块的范围的错误严重级别",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProcName",
                table: "ErrorLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "返回存储过程或触发器的名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurDate",
                table: "ErrorLogs",
                type: "datetime2",
                nullable: false,
                comment: "错误发生时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                comment: "错误码",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ErrorLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "返回特定于它被引用 CATCH 块作用域的错误消息",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Line",
                table: "ErrorLogs",
                type: "int",
                nullable: false,
                comment: "返回特定于引用它的 CATCH 块作用域的错误行号",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InParams",
                table: "ErrorLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "传入参数",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "DriverConfigs",
                type: "nvarchar(max)",
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DriverConfigs",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "DriverConfigs",
                type: "uniqueidentifier",
                nullable: false,
                comment: "设备唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DriverConfigs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebPort",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "端口",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "服务编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "IpAdress",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "IP地址",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DmpMachines",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DmpId",
                table: "DmpMachines",
                type: "int",
                nullable: false,
                comment: "DMP Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DeviceGroupYieldMachines",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "DeviceGroupYieldMachines",
                type: "int",
                nullable: false,
                comment: "设备组Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "DeviceGroups",
                type: "int",
                nullable: false,
                comment: "显示顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "DeviceGroups",
                type: "int",
                nullable: true,
                comment: "父节点Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpGroupId",
                table: "DeviceGroups",
                type: "uniqueidentifier",
                nullable: false,
                comment: "设备组唯一表示 GUID",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "DeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "设备组名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "设备组编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "DeviceGroupPermissions",
                type: "int",
                nullable: true,
                comment: "租户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsGranted",
                table: "DeviceGroupPermissions",
                type: "bit",
                nullable: false,
                comment: "是否赋予权限",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "DeviceGroupPermissions",
                type: "int",
                nullable: false,
                comment: "设备组Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DefectiveReasons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "不良原因名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "DefectiveReasons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DefectiveReasons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "不良原因编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "DefectiveParts",
                type: "int",
                nullable: true,
                comment: "上级组ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DefectiveParts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "不良原部位名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DefectiveParts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "不良部位编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ReasonId",
                table: "DefectivePartReasons",
                type: "int",
                nullable: false,
                comment: "不良原因ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PartId",
                table: "DefectivePartReasons",
                type: "int",
                nullable: false,
                comment: "不良部位ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PartCode",
                table: "DefectivePartReasons",
                type: "nvarchar(max)",
                nullable: true,
                comment: "不良部位编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "总共时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StopDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "停机时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "DailyStatesSummaries",
                type: "datetime2",
                nullable: false,
                comment: "工程日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "RunDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "运行时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfflineDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "离线时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "DailyStatesSummaries",
                type: "int",
                nullable: true,
                comment: "班次key",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "DailyStatesSummaries",
                type: "bigint",
                nullable: false,
                comment: "设备key",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifyTime",
                table: "DailyStatesSummaries",
                type: "datetime2",
                nullable: false,
                comment: "最后修改时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "FreeDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "空闲时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DebugDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                comment: "调试时长",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "PId",
                table: "CutterTypes",
                type: "int",
                nullable: true,
                comment: "父节点Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCutterNoPrefixCanEdit",
                table: "CutterTypes",
                type: "bit",
                nullable: false,
                comment: "刀具编号前缀是否能编辑",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNoPrefix",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "刀具编号前缀",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "WarningLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "预警寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UsedLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "使用寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RestLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "剩余寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Rate",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "寿命消耗倍率",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数9",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数8",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数7",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数6",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数5",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数4",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数3",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数2",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数10",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数1",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "原始寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "CutterStates",
                type: "int",
                nullable: true,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CutterUsedStatus",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "刀具使用状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "刀具类型Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTValue",
                table: "CutterStates",
                type: "int",
                nullable: true,
                comment: "刀位",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "刀具编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CutterModelId",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "刀具型号Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CutterLifeStatus",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "刀具寿命状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CountingMethod",
                table: "CutterStates",
                type: "int",
                nullable: false,
                comment: "计数方式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CutterParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "WarningLife",
                table: "CutterModels",
                type: "int",
                nullable: false,
                comment: "预警寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数9",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数8",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数7",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数6",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数5",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数4",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数3",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数2",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数10",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数1",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterModels",
                type: "int",
                nullable: false,
                comment: "初始寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterModels",
                type: "int",
                nullable: false,
                comment: "刀具类型Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNoPrefix",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "刀具编号前缀",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "UsedLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "已用寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RestLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "剩余寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数9",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数8",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数7",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数6",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数5",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数4",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数3",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数2",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数10",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "刀具参数1",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "原始寿命",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "OperatorUserId",
                table: "CutterLoadAndUnloadRecords",
                type: "bigint",
                nullable: true,
                comment: "操作人",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OperatorTime",
                table: "CutterLoadAndUnloadRecords",
                type: "datetime2",
                nullable: true,
                comment: "操作时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OperationType",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "0：卸刀，1：装刀",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "刀具类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTValue",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: true,
                comment: "刀位（刀具T值）",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "刀具编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CutterModelId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "刀具型号",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CountingMethod",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                comment: "寿命计数方式（0：按次数，1：按时间）",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RenderHtml",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                comment: "渲染Dom节点",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CustomFields",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "MaxLength",
                table: "CustomFields",
                type: "int",
                nullable: false,
                comment: "最大长度",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRequired",
                table: "CustomFields",
                type: "bit",
                nullable: false,
                comment: "是否必需",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "HtmlTemplate",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                comment: "编辑节点",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayType",
                table: "CustomFields",
                type: "int",
                nullable: false,
                comment: "显示类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CustomFields",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Crafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "工艺名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Crafts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "工艺备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Crafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "工艺代码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProcessOrder",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                comment: "工艺加工顺序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessName",
                table: "CraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "工艺名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                comment: "工艺ID",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessCode",
                table: "CraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "工艺代码",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLastProcess",
                table: "CraftProcesses",
                type: "bit",
                nullable: false,
                comment: "是否是最后工序",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                comment: "产品工艺Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ChatMessages",
                type: "bigint",
                nullable: false,
                comment: "用户Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "ChatMessages",
                type: "int",
                nullable: true,
                comment: "租户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TargetUserId",
                table: "ChatMessages",
                type: "bigint",
                nullable: false,
                comment: "目标用户Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "TargetTenantId",
                table: "ChatMessages",
                type: "int",
                nullable: true,
                comment: "目标租户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Side",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                comment: "发送方/接收方",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "SharedMessageId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true,
                comment: "唯一标识(GUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverReadState",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                comment: "接收方是否已读",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReadState",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                comment: "是否已读",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                comment: "信息",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "PrinterName",
                table: "CartonSettings",
                type: "nvarchar(max)",
                nullable: true,
                comment: "打印机名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxPackingCount",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                comment: "最大装箱数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnpackingRedo",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "是否允许拆箱重装",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnpackingAfterPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "是否允许打印后拆箱重装",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "是否打印",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsGoodOnly",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "只能装合格件",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinalTest",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "是否需要终检",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "是否自动打印",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "HasToFlow",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "必经流程",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "ForbidRepeatPacking",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "禁止重复装箱",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "ForbidHopSequence",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "禁止跳序",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FlowIds",
                table: "CartonSettings",
                type: "nvarchar(max)",
                nullable: true,
                comment: "必经流程Id",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                comment: "设备组Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                comment: "箱号规则Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "AutoCartonNo",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                comment: "自动生成箱码",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CartonSerialNumbers",
                type: "int",
                nullable: false,
                comment: "状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "CartonSerialNumbers",
                type: "nvarchar(max)",
                nullable: true,
                comment: "序列号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "CartonSerialNumbers",
                type: "int",
                nullable: false,
                comment: "日期 20230106",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RealPackingCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                comment: "实际包装数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PrintLabelCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                comment: "打印次数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPackingCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                comment: "最大包装数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "Cartons",
                type: "int",
                nullable: false,
                comment: "设备组Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CartonNo",
                table: "Cartons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "箱号",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CartonRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CartonRules",
                type: "bit",
                nullable: false,
                comment: "是否启用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CartonRuleDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "类型",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StartIndex",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "开始位",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SequenceNo",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "顺序号",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "CartonRuleDetails",
                type: "nvarchar(max)",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Length",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "长度",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExpansionKey",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "扩展键",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EndIndex",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "结束位",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                comment: "箱号规则Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "CartonRecords",
                type: "int",
                nullable: false,
                comment: "具体班次Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "CartonRecords",
                type: "datetime2",
                nullable: false,
                comment: "班次日",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "CartonRecords",
                type: "nvarchar(max)",
                nullable: false,
                comment: "工件二维码",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CartonNo",
                table: "CartonRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "箱号",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "CartonId",
                table: "CartonRecords",
                type: "int",
                nullable: false,
                comment: "箱Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Yield",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                comment: "产量",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "用户班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "用户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "工件状态",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                comment: "人员班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                comment: "班次方案名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                comment: "班次日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                comment: "循环倍率",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "Qualified",
                table: "Capacities",
                type: "bit",
                nullable: true,
                comment: "质量",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "Capacities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "程序名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Capacities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "产品名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Capacities",
                type: "int",
                nullable: false,
                comment: "产品Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousLinkId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: true,
                comment: "连接前面一笔记录的DmpId",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlanName",
                table: "Capacities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "生产计划名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "生产计划Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanAmount",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "生产计划目标产量",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "零件码",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalCount",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                comment: "原始计数器",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "工单Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "Mongo同步时间",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Capacities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "设备班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "Capacities",
                type: "bigint",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "Capacities",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsValid",
                table: "Capacities",
                type: "bit",
                nullable: false,
                comment: "是否有效记录",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLineOutputOffline",
                table: "Capacities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "产线产量是否下线",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLineOutput",
                table: "Capacities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否是产线产出",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "Capacities",
                type: "bigint",
                nullable: false,
                comment: "加工时长(秒)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: false,
                comment: "记录DmpId，用于内部计算使用",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Capacities",
                type: "int",
                nullable: true,
                comment: "系统日历key",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AccumulateCount",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                comment: "累积计数器",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CalibratorCodes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "Key",
                table: "CalibratorCodes",
                type: "int",
                nullable: false,
                comment: "键",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CalibratorCodes",
                type: "int",
                nullable: false,
                comment: "包装规则Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Calendars",
                type: "int",
                nullable: false,
                comment: "当前年第几周（每年1月1日作为第一周）",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "YYYYWeek",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                comment: "格式化显示(年-周)：2017-01",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "YYYYMM",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                comment: "格式化显示（年-月）：2017-01",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "YYYYISOWeek",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                comment: "格式化显示(ISO标准，年-周)：2017-01",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<byte>(
                name: "Weekday",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前周第几天：5",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "WeekOfYear",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前周第几天",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "WeekOfMonth",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前月第几周：3",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "WeekDayName",
                table: "Calendars",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "星期几，英文：Wednesday",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "QuarterName",
                table: "Calendars",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                comment: "季度名称，英文：First",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<byte>(
                name: "Quarter",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "季度：2",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "MonthYear",
                table: "Calendars",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                comment: "格式化显示（月缩写-年）Jan2017",
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "MonthName",
                table: "Calendars",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "月份名称，英文：January",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<byte>(
                name: "Month",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "月份",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "MMYYYY",
                table: "Calendars",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                comment: "格式化显示（月年）：012017",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前年最后一天：2017-12-31（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfQuarter",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前季度最后一天：2017-03-31（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前月最后一天：2017/1/31（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWorkday",
                table: "Calendars",
                type: "bit",
                nullable: false,
                comment: "是否是工作日；1是",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWeekend",
                table: "Calendars",
                type: "bit",
                nullable: false,
                comment: "是否是礼拜日；1是",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHoliday",
                table: "Calendars",
                type: "bit",
                nullable: false,
                comment: "是否是假期；1是",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "ISOWeekOfYear",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前年第几周（ISO标准，每年1月4日所在周作为第一周）",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "HolidayText",
                table: "Calendars",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "假期名称，此字段目前未使用。礼拜日，工作日，假期字段一般会在另外的工厂日历中",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前年第一天：2017-01-01（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfQuarter",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前季度第一天：2017-01-01（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfNextYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "下一个年第一天：2018-01-01（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfNextMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "下个月第一天：2017-02-01（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "当前月第一天：2017/1/1（日期类型，需要格式化）",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "DaySuffix",
                table: "Calendars",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                comment: "英文后缀,th",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<short>(
                name: "DayOfYear",
                table: "Calendars",
                type: "smallint",
                nullable: false,
                comment: "当前年第几天",
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<byte>(
                name: "Day",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前月第几天",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Calendars",
                type: "date",
                nullable: false,
                comment: "自然日",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<byte>(
                name: "DOWInMonth",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                comment: "当前月第几周(每月第一天是第一周的第一天)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Calendars",
                type: "int",
                nullable: false,
                comment: "自然日数字格式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TargetTable",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档源表表名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档目标表名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedMessage",
                table: "ArchiveEntries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "归档结果信息",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArchiveValue",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档列内容",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ArchiveTotalCount",
                table: "ArchiveEntries",
                type: "bigint",
                nullable: false,
                comment: "归档总计数量",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ArchiveCount",
                table: "ArchiveEntries",
                type: "bigint",
                nullable: false,
                comment: "归档最后一次数据数量",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ArchiveColumn",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "归档列名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                comment: "租户Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                comment: "支付状态",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentPeriodType",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: true,
                comment: "支付时长 月/年",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentId",
                table: "AppSubscriptionPayments",
                type: "nvarchar(max)",
                nullable: true,
                comment: "支付Id",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "AppSubscriptionPayments",
                type: "nvarchar(max)",
                nullable: true,
                comment: "发票号",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gateway",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                comment: "支付方式",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                comment: "版本Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DayCount",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                comment: "日数量",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "AppSubscriptionPayments",
                type: "decimal(18,2)",
                nullable: false,
                comment: "数量",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "AppBinaryObjects",
                type: "int",
                nullable: true,
                comment: "租户key",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Bytes",
                table: "AppBinaryObjects",
                type: "varbinary(max)",
                nullable: false,
                comment: "字节数据，存储图片或文件等的字节数据",
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "用户班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "用户Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                comment: "开始时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "人员班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "班次方案名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                comment: "班次日期",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                comment: "设备班次名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "Alarms",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "程序名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Alarms",
                type: "int",
                nullable: false,
                comment: "产品Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "工序Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "工件编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "工单Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "Mongo同步时间",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alarms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "报警内容",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Alarms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "备注，报警信息手动维护在该字段",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "设备班次Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "Alarms",
                type: "bigint",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "Alarms",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "设备编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                comment: "结束时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "Alarms",
                type: "decimal(18,2)",
                nullable: false,
                comment: "持续时长（秒）",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Alarms",
                type: "int",
                nullable: true,
                comment: "系统日历Key",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "报警编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "AlarmInfos",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                comment: "报警原因",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AlarmInfos",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                comment: "报警内容",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "AlarmInfos",
                type: "int",
                nullable: false,
                comment: "设备Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AlarmInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "报警编号",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<int>(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                comment: "到期后 等待续费日期",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrialDayCount",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                comment: "到期日",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true,
                comment: "月度价格",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExpiringEditionId",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                comment: "到期版本Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AnnualPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true,
                comment: "年度价格",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工单key");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "WorkOrderTasks",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "WorkOrderTasks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "合格数量");

            migrationBuilder.AlterColumn<int>(
                name: "OutputCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "输出数量");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "WorkOrderTasks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "WorkOrderTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不合格数量");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "合格数量");

            migrationBuilder.AlterColumn<int>(
                name: "PutVolume",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设置值");

            migrationBuilder.AlterColumn<int>(
                name: "ProductionPlanId",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "生产计划key");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工序key");

            migrationBuilder.AlterColumn<int>(
                name: "OutputCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "输出数量");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLastProcessOrder",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否最后工序");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不合格数量");

            migrationBuilder.AlterColumn<decimal>(
                name: "CompletionRate",
                table: "WorkOrders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "完成率");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编码");

            migrationBuilder.AlterColumn<int>(
                name: "AimVolume",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "目标值");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderTaskId",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工单任务key");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonsId",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良原因key");

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "WorkOrderDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数量");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "WeChatNotifications",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户key");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationTypeId",
                table: "WeChatNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "通知消息类型Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "WeChatNotifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启用（1启用,0未启用）");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingStationDisplayName",
                table: "TraceRelatedMachines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "工位名称");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingStationCode",
                table: "TraceRelatedMachines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "工位编号");

            migrationBuilder.AlterColumn<int>(
                name: "TraceFlowSettingId",
                table: "TraceRelatedMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程设定ID");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "TraceRelatedMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备ID");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "TraceRelatedMachines",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<bool>(
                name: "WriteIntoPlcViaFlowData",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "数据触发时写信号");

            migrationBuilder.AlterColumn<bool>(
                name: "WriteIntoPlcViaFlow",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "流程触发时写信号");

            migrationBuilder.AlterColumn<int>(
                name: "TriggerEndFlowStyle",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "结束流程方式");

            migrationBuilder.AlterColumn<int>(
                name: "StationType",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工位类型");

            migrationBuilder.AlterColumn<int>(
                name: "SourceOfPartNo",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工件来源");

            migrationBuilder.AlterColumn<int>(
                name: "QualityMakerFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "质量责任归属到特定流程");

            migrationBuilder.AlterColumn<int>(
                name: "PreFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "上一流程ID");

            migrationBuilder.AlterColumn<int>(
                name: "NextFlowId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "下一流程ID");

            migrationBuilder.AlterColumn<bool>(
                name: "NeedHandlerRelateData",
                table: "TraceFlowSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否处理相关数据");

            migrationBuilder.AlterColumn<int>(
                name: "FlowType",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程类别");

            migrationBuilder.AlterColumn<int>(
                name: "FlowSeq",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程顺序");

            migrationBuilder.AlterColumn<string>(
                name: "ExtensionData",
                table: "TraceFlowSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "数据");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "TraceFlowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "流程名称");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "TraceFlowSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组ID");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TraceFlowSettings",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "流程编号");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "TraceFlowRecords",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户ID");

            migrationBuilder.AlterColumn<int>(
                name: "TraceFlowSettingId",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程设定ID");

            migrationBuilder.AlterColumn<int>(
                name: "Tag",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程标识");

            migrationBuilder.AlterColumn<string>(
                name: "Station",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "工位");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "流程状态");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "TraceFlowRecords",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "工件编号");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "TraceFlowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备ID");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeftTime",
                table: "TraceFlowRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "离开时间");

            migrationBuilder.AlterColumn<string>(
                name: "FlowDisplayName",
                table: "TraceFlowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "流程名称");

            migrationBuilder.AlterColumn<string>(
                name: "FlowCode",
                table: "TraceFlowRecords",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "流程编号");

            migrationBuilder.AlterColumn<string>(
                name: "ExtensionData",
                table: "TraceFlowRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "数据");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EntryTime",
                table: "TraceFlowRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "进入时间");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "TraceFlowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档表");

            migrationBuilder.AlterColumn<bool>(
                name: "Qualified",
                table: "TraceCatalogs",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "是否合格");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计划Id");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "TraceCatalogs",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "工件编号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineTime",
                table: "TraceCatalogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "上线时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfflineTime",
                table: "TraceCatalogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "下线时间");

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备班次详情Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReworkPart",
                table: "TraceCatalogs",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "是否返工");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "TraceCatalogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组ID");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "TraceCatalogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档表");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramF",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号F");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramE",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号E");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramD",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号D");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramC",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号C");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramB",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号B");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramA",
                table: "Tongs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "程序号A");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Tongs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tongs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tongs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Tongs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "容量");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ThirdpartyApis",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "地址");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "ThirdpartyApis",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "类型");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ThirdpartyApis",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "名字");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ThirdpartyApis",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "编码");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionEndDateUtc",
                table: "Tenants",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "订阅结束日期Utc");

            migrationBuilder.AlterColumn<Guid>(
                name: "LogoId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Logo Id");

            migrationBuilder.AlterColumn<string>(
                name: "LogoFileType",
                table: "Tenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "Logo文件类型");

            migrationBuilder.AlterColumn<bool>(
                name: "IsInTrialPeriod",
                table: "Tenants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否正在试用期间");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomCssId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "自定义样式Id");

            migrationBuilder.AlterColumn<int>(
                name: "TotalCount",
                table: "SyncDataFlags",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "总共同步数量");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessName",
                table: "SyncDataFlags",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "作业名称");

            migrationBuilder.AlterColumn<string>(
                name: "LastSyncTime",
                table: "SyncDataFlags",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "上次同步时间");

            migrationBuilder.AlterColumn<int>(
                name: "LastSyncCount",
                table: "SyncDataFlags",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "上次同步数量");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "States",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "班次开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "人员班次名称");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "班次方案名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "States",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "班次日期");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "States",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备班次名称");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "States",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "程序号");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "States",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品Id");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousLinkId",
                table: "States",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "连接前面一笔记录的DmpId");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "零件码");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工单Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "状态名称");

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "创建时间 记录在MongoDb");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "States",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "设备班次Id");

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "States",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "States",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShiftSplit",
                table: "States",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否切班产生");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "States",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "States",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "持续时长（秒）");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpId",
                table: "States",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "唯一标识(GUID)");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "States",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "系统日历 20230106");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "States",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "状态编号");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "StateInfos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态类型");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalCode",
                table: "StateInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "原始采集编号");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStatic",
                table: "StateInfos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "静态状态无法编辑和删除，由程序创建");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaned",
                table: "StateInfos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否计划内");

            migrationBuilder.AlterColumn<string>(
                name: "Hexcode",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "状态颜色");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "状态名称");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "StateInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "状态编号");

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardCostTime",
                table: "StandardTime",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "用时");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "StandardTime",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品Id");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "StandardTime",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "StandardTime",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "CycleRate",
                table: "StandardTime",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "倍率");

            migrationBuilder.AlterColumn<int>(
                name: "TargetYiled",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "目标产量");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftTargetYileds",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次日");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShiftTargetYileds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品key");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShiftSolutions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "班次方案名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ShiftSolutionItems",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftSolutionItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShiftSolutionItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "班次名称");

            migrationBuilder.AlterColumn<bool>(
                name: "IsNextDay",
                table: "ShiftSolutionItems",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "班次是否跨天");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ShiftSolutionItems",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "ShiftSolutionItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "班次持续时间 秒");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "具体班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftHistories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次日");

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次日期");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ShiftHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftYearName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "班次年名称");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftYear",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次年");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftWeekName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "班次周名称 2020-03 周");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftWeek",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次周");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftMonthName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "班次月名称 2020-01 月");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftMonth",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次月");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftItemSeq",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "具体班次顺序");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftItemId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "具体班次Id");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDayName",
                table: "ShiftCalendars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "班次日名称 2020-02-08 日");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次日");

            migrationBuilder.AlterColumn<int>(
                name: "MachineShiftDetailId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ShiftCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ShiftCalendars",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次结束时间");

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "ShiftCalendars",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "班次时长，秒");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "维修开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "RequestUserId",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "申请人");

            migrationBuilder.AlterColumn<string>(
                name: "RequestMemo",
                table: "RepairRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "请求备注");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "RepairRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "申请日期");

            migrationBuilder.AlterColumn<int>(
                name: "RepairUserId",
                table: "RepairRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "维修人");

            migrationBuilder.AlterColumn<string>(
                name: "RepairMemo",
                table: "RepairRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "维修备注");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RepairDate",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "维修日期");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "RepairRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShutdown",
                table: "RepairRequests",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否关机");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "RepairRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "维修结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "RepairRequests",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "耗费时间");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RepairRequests",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态Id");

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "ReasonFeedbackRecords",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "状态编号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "ReasonFeedbackRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "EndUserId",
                table: "ReasonFeedbackRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "结束用户Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "ReasonFeedbackRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "ReasonFeedbackRecords",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "持续时间");

            migrationBuilder.AlterColumn<string>(
                name: "Spec",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "规格");

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品组key");

            migrationBuilder.AlterColumn<string>(
                name: "PartType",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "零件类型");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<string>(
                name: "IsHalfFinished",
                table: "Products",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "是否半成品");

            migrationBuilder.AlterColumn<string>(
                name: "DrawingNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "图纸号");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "描述");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "编码");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "ProductionPlans",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "单位");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "QualifiedCount",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "合格数量");

            migrationBuilder.AlterColumn<int>(
                name: "PutVolume",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设置值");

            migrationBuilder.AlterColumn<int>(
                name: "ProductionPlanState",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "生产计划状态");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品key");

            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "工序编码");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "ProductionPlans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveCount",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良数量");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工艺key");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编码");

            migrationBuilder.AlterColumn<string>(
                name: "ClientName",
                table: "ProductionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "客户端名称");

            migrationBuilder.AlterColumn<int>(
                name: "AimVolume",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "目标值");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualStartDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "实际开始时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualEndDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "实际结束时间");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名字");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "ProductGroups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "描述");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "编码");

            migrationBuilder.AlterColumn<int>(
                name: "YieldSummaryType",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产量计算方式");

            migrationBuilder.AlterColumn<int>(
                name: "YieldCounterMachineId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产量计数器设备");

            migrationBuilder.AlterColumn<int>(
                name: "TargetType",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "目标量维度");

            migrationBuilder.AlterColumn<int>(
                name: "TargetAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "目标量");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计划状态");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RealStartTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "实际开始时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RealEndTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "实际截止时间");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "产品名称");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品ID");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "已完成量");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlanStartTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "计划开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "PlanName",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "计划名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlanEndTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "计划截止时间");

            migrationBuilder.AlterColumn<string>(
                name: "PlanCode",
                table: "ProcessPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "计划编号");

            migrationBuilder.AlterColumn<int>(
                name: "PlanAmount",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计划产量");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PauseTime",
                table: "ProcessPlans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "暂停时间");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTimeRangeSelect",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "时间范围是否选中");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoStartNextPlan",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "自动开启后续计划");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoFinishCurrentPlan",
                table: "ProcessPlans",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "计划生产量达标后自动关闭计划");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "ProcessPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组ID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Process",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Process",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "描述");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Process",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编码");

            migrationBuilder.AlterColumn<int>(
                name: "SolutionId",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案ID");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftTargetAmount",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次目标量");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftName",
                table: "PlanTargets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "班次名称");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "PlanTargets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次ID");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "PerformancePersonnelOnDevices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "上线时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineDate",
                table: "PerformancePersonnelOnDevices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "设备key");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "PerformancePersonnelOnDevices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "用户Key");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "PartDefects",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "零件编号");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良原因ID");

            migrationBuilder.AlterColumn<int>(
                name: "DefectivePartId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良部位ID");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveMachineId",
                table: "PartDefects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良设备ID");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "OnlineAndOfflineLogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户Key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OnlineDateTime",
                table: "OnlineAndOfflineLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "上线时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfflineDateTime",
                table: "OnlineAndOfflineLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "下线时间");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "OnlineAndOfflineLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备key");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "NotificationTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "父节点Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Code，参照部门实现方式");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "NotificationTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "用户key");

            migrationBuilder.AlterColumn<int>(
                name: "TriggerType",
                table: "NotificationRules",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "触发类型");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationRules",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "规则名称");

            migrationBuilder.AlterColumn<int>(
                name: "MessageType",
                table: "NotificationRules",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "消息类型");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceGroupIds",
                table: "NotificationRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "设备组Ids");

            migrationBuilder.AlterColumn<int>(
                name: "TriggerCondition",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "触发条件");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationRuleId",
                table: "NotificationRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "通知规则Id");

            migrationBuilder.AlterColumn<string>(
                name: "NoticeUserIds",
                table: "NotificationRuleDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "通知人员Id(1,2,3)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "NotificationRuleDetails",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启用");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "消息状态(0:未发送,1:已发送)");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationType",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "通知方式（微信，邮件）");

            migrationBuilder.AlterColumn<long>(
                name: "NoticedUserId",
                table: "NotificationRecords",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "通知人Id");

            migrationBuilder.AlterColumn<int>(
                name: "MessageType",
                table: "NotificationRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "消息类型");

            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "NotificationRecords",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true,
                oldComment: "消息内容");

            migrationBuilder.AlterColumn<string>(
                name: "RootDeviceGroupCode",
                table: "Notices",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "车间代码");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Notices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启用");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "公告内容");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "MaintainPlans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "计划生效日期");

            migrationBuilder.AlterColumn<int>(
                name: "PersonInChargeId",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "负责人");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintainPlans",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "MaintainPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<int>(
                name: "IntervalDate",
                table: "MaintainPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "间隔日期");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "MaintainPlans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "计划失效日期");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MaintainPlans",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "计划保养日期");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "MaintainOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "MaintainUserId",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "保养用户Id");

            migrationBuilder.AlterColumn<string>(
                name: "MaintainPlanCode",
                table: "MaintainOrders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "保养计划编号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MaintainDate",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "实际保养日期");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MaintainOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MaintainOrders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "MaintainOrders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "耗费时间");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MaintainOrders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "编号");

            migrationBuilder.AlterColumn<double>(
                name: "ValueFactor",
                table: "MachineVariables",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "值类型");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "类型");

            migrationBuilder.AlterColumn<string>(
                name: "Methodparam",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "方法参数");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "Index",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "顺序");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineVariables",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "设备唯一标识(GUID)");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceAddress",
                table: "MachineVariables",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "设备地址");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "描述");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultValue",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "默认值");

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "数据类型");

            migrationBuilder.AlterColumn<int>(
                name: "DataLength",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数据长度");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MachineVariables",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "Access",
                table: "MachineVariables",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "存取");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备类型名称");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "MachineTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备类型描述");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "具体班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "MachinesShiftDetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次日");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachinesShiftDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "MachineShiftEffectiveIntervals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachineShiftEffectiveIntervals",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineShiftEffectiveIntervals",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MachineShiftEffectiveIntervals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "操作类型");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionId",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次方案Id");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "显示顺序");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineShiftChangeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "UId",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "硬件用户名");

            migrationBuilder.AlterColumn<int>(
                name: "SortSeq",
                table: "Machines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "显示顺序");

            migrationBuilder.AlterColumn<string>(
                name: "ProductKey",
                table: "Machines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "弃用");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "硬件密码");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "设备名称");

            migrationBuilder.AlterColumn<int>(
                name: "MachineTypeId",
                table: "Machines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备类型Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCalLineCapacity",
                table: "Machines",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "弃用");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Machines",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启用");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                table: "Machines",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "图片Id(GUID)，在表AppBinaryObjects中");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "Machines",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "唯一标识(GUID)");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceSecret",
                table: "Machines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "弃用");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Machines",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "弃用");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Machines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "设备描述");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Machines",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramNo",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "程序号");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessCode",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "工序编号");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachinePrograms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachinePrograms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品key");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工序key");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "MachineProcesses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "ChangeProductUserId",
                table: "MachineProcesses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "更改产品的用户key");

            migrationBuilder.AlterColumn<int>(
                name: "TcpPort",
                table: "MachineNetConfigs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "端口");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachineNetConfigs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "MachineNetConfigs",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true,
                oldComment: "IP地址");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineNetConfigs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "设备唯一标识(GUID)");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "参数单位");

            migrationBuilder.AlterColumn<int>(
                name: "SortSeq",
                table: "MachineGatherParams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "显示顺序");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "参数名称");

            migrationBuilder.AlterColumn<double>(
                name: "Min",
                table: "MachineGatherParams",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "参数最小值");

            migrationBuilder.AlterColumn<double>(
                name: "Max",
                table: "MachineGatherParams",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "参数最大值");

            migrationBuilder.AlterColumn<Guid>(
                name: "MachineVariableId",
                table: "MachineGatherParams",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "唯一标识(GUID)");

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "MachineGatherParams",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "MachineGatherParams",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForVisual",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "是否在看板显示");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForStatus",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "是否在实时状态页显示");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowForParam",
                table: "MachineGatherParams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "是否在运行参数页显示");

            migrationBuilder.AlterColumn<string>(
                name: "Hexcode",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "参数显示颜色");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayStyle",
                table: "MachineGatherParams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "参数显示方式");

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "MachineGatherParams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "参数数据类型");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "MachineGatherParams",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "参数编号");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "MachineDrivers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "类型名称");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDrivers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<bool>(
                name: "Enable",
                table: "MachineDrivers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启动");

            migrationBuilder.AlterColumn<string>(
                name: "Driver",
                table: "MachineDrivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "驱动");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "MachineDrivers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "设备唯一标识(GUID)");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDeviceGroups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "MachineDeviceGroups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组Id");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceGroupCode",
                table: "MachineDeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "设备组编号");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "班次信息ID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "MachineDefectiveRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "日期");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品ID");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备ID");

            migrationBuilder.AlterColumn<int>(
                name: "DefectiveReasonsId",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良原因ID");

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "MachineDefectiveRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数量");

            migrationBuilder.AlterColumn<string>(
                name: "TenantTaxNo",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "租客号码");

            migrationBuilder.AlterColumn<string>(
                name: "TenantLegalName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "法定名称");

            migrationBuilder.AlterColumn<string>(
                name: "TenantAddress",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "租户地址");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "发票号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "发票日期");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "InfoLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "ProcName",
                table: "InfoLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "执行存储过程名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "InfoLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "InfoLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "执行时长，秒");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InfoLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "GUID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StratTime",
                table: "InfoLogDetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "Step",
                table: "InfoLogDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "步骤");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "InfoLogDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "信息");

            migrationBuilder.AlterColumn<Guid>(
                name: "InfoLogId",
                table: "InfoLogDetails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "InfoLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "InfoLogDetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "InfoLogDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "执行时长，秒");

            migrationBuilder.AlterColumn<int>(
                name: "AffectedRowCount",
                table: "InfoLogDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "影响的行数");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Friendships",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户Id");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Friendships",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "租户Id");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Friendships",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<string>(
                name: "FriendUserName",
                table: "Friendships",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "关联用户名");

            migrationBuilder.AlterColumn<long>(
                name: "FriendUserId",
                table: "Friendships",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "关联用户Id");

            migrationBuilder.AlterColumn<int>(
                name: "FriendTenantId",
                table: "Friendships",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "关联租户Id");

            migrationBuilder.AlterColumn<string>(
                name: "FriendTenancyName",
                table: "Friendships",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "关联租户名");

            migrationBuilder.AlterColumn<Guid>(
                name: "FriendProfilePictureId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "关联用户图片标识");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "FmsCutterSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "字段类型");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "FmsCutterSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "顺序");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow",
                table: "FmsCutterSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否显示");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "FmsCutterSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "编号");

            migrationBuilder.AlterColumn<decimal>(
                name: "WarningLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "预警寿命");

            migrationBuilder.AlterColumn<int>(
                name: "UseType",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "使用类型");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "刀具类型");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具状态");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "预设寿命");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "FmsCutters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "LengthCompensate",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "长度补偿");

            migrationBuilder.AlterColumn<decimal>(
                name: "Length",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "长度");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiameterCompensate",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "直径补偿");

            migrationBuilder.AlterColumn<decimal>(
                name: "Diameter",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "直径");

            migrationBuilder.AlterColumn<string>(
                name: "CutterStockNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "刀库编号");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "刀具编号");

            migrationBuilder.AlterColumn<string>(
                name: "CutterCase",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "刀套编号");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentLife",
                table: "FmsCutters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "当前寿命");

            migrationBuilder.AlterColumn<int>(
                name: "CountType",
                table: "FmsCutters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计数类型");

            migrationBuilder.AlterColumn<string>(
                name: "CompensateNo",
                table: "FmsCutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "补偿号");

            migrationBuilder.AlterColumn<int>(
                name: "FmsCutterId",
                table: "FmsCutterExtends",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具Id");

            migrationBuilder.AlterColumn<string>(
                name: "FieldValue",
                table: "FmsCutterExtends",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "字段值");

            migrationBuilder.AlterColumn<int>(
                name: "CustomFieldId",
                table: "FmsCutterExtends",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "自定义字段Id");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "FlexibleCrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "版本--产品下的版本号唯一");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "FlexibleCrafts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FlexibleCrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名称");

            migrationBuilder.AlterColumn<int>(
                name: "TongId",
                table: "FlexibleCraftProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "夹具Id");

            migrationBuilder.AlterColumn<int>(
                name: "Sequence",
                table: "FlexibleCraftProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "顺序号");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FlexibleCraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名称");

            migrationBuilder.AlterColumn<int>(
                name: "CraftProcesseId",
                table: "FlexibleCraftProcesseMaps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "FlexibleCraftProcesseMaps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工艺Id");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureNumber",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "程序号");

            migrationBuilder.AlterColumn<int>(
                name: "CutterId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具Id");

            migrationBuilder.AlterColumn<int>(
                name: "CraftProcesseId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "FlexibleCraftProcedureCutterMaps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工艺Id");

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "状态编号");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "名称");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "FeedbackCalendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "持续时间 分钟");

            migrationBuilder.AlterColumn<string>(
                name: "Cron",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "表达式");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "FeedbackCalendars",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "FeedbackCalendarDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "FeedbackCalendarId",
                table: "FeedbackCalendarDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "反馈频率Id");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "返回引用它的 CATCH 块范围特有的错误状态");

            migrationBuilder.AlterColumn<int>(
                name: "Serverity",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "返回特定于引用它的 CATCH 块的范围的错误严重级别");

            migrationBuilder.AlterColumn<string>(
                name: "ProcName",
                table: "ErrorLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "返回存储过程或触发器的名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurDate",
                table: "ErrorLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "错误发生时间");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "ErrorLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "错误码");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ErrorLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true,
                oldComment: "返回特定于它被引用 CATCH 块作用域的错误消息");

            migrationBuilder.AlterColumn<int>(
                name: "Line",
                table: "ErrorLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "返回特定于引用它的 CATCH 块作用域的错误行号");

            migrationBuilder.AlterColumn<string>(
                name: "InParams",
                table: "ErrorLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "传入参数");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "DriverConfigs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "值");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DriverConfigs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpMachineId",
                table: "DriverConfigs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "设备唯一标识(GUID)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DriverConfigs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "编号");

            migrationBuilder.AlterColumn<string>(
                name: "WebPort",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "端口");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "服务编号");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "IpAdress",
                table: "Dmps",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "IP地址");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DmpMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "DmpId",
                table: "DmpMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "DMP Id");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "DeviceGroupYieldMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "DeviceGroupYieldMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组Id");

            migrationBuilder.AlterColumn<int>(
                name: "Seq",
                table: "DeviceGroups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "显示顺序");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "DeviceGroups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "父节点Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpGroupId",
                table: "DeviceGroups",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "设备组唯一表示 GUID");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "DeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "设备组名称");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DeviceGroups",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "设备组编号");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "DeviceGroupPermissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "租户Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsGranted",
                table: "DeviceGroupPermissions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否赋予权限");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "DeviceGroupPermissions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DefectiveReasons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "不良原因名称");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "DefectiveReasons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DefectiveReasons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "不良原因编号");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "DefectiveParts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "上级组ID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DefectiveParts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "不良原部位名称");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DefectiveParts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "不良部位编号");

            migrationBuilder.AlterColumn<int>(
                name: "ReasonId",
                table: "DefectivePartReasons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良原因ID");

            migrationBuilder.AlterColumn<int>(
                name: "PartId",
                table: "DefectivePartReasons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "不良部位ID");

            migrationBuilder.AlterColumn<string>(
                name: "PartCode",
                table: "DefectivePartReasons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "不良部位编号");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "总共时长");

            migrationBuilder.AlterColumn<decimal>(
                name: "StopDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "停机时长");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "DailyStatesSummaries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "工程日期");

            migrationBuilder.AlterColumn<decimal>(
                name: "RunDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "运行时长");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfflineDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "离线时长");

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "DailyStatesSummaries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "班次key");

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "DailyStatesSummaries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifyTime",
                table: "DailyStatesSummaries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "最后修改时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "FreeDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "空闲时长");

            migrationBuilder.AlterColumn<decimal>(
                name: "DebugDuration",
                table: "DailyStatesSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "调试时长");

            migrationBuilder.AlterColumn<int>(
                name: "PId",
                table: "CutterTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "父节点Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "名称");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCutterNoPrefixCanEdit",
                table: "CutterTypes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "刀具编号前缀是否能编辑");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNoPrefix",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "刀具编号前缀");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CutterTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "WarningLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "预警寿命");

            migrationBuilder.AlterColumn<int>(
                name: "UsedLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "使用寿命");

            migrationBuilder.AlterColumn<int>(
                name: "RestLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "剩余寿命");

            migrationBuilder.AlterColumn<int>(
                name: "Rate",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "寿命消耗倍率");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数9");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数8");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数7");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数6");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数5");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数4");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数3");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数2");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数10");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数1");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "原始寿命");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "CutterStates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "CutterUsedStatus",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具使用状态");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具类型Id");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTValue",
                table: "CutterStates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "刀位");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "CutterStates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "刀具编号");

            migrationBuilder.AlterColumn<int>(
                name: "CutterModelId",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具型号Id");

            migrationBuilder.AlterColumn<int>(
                name: "CutterLifeStatus",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具寿命状态");

            migrationBuilder.AlterColumn<int>(
                name: "CountingMethod",
                table: "CutterStates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计数方式");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "名称");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CutterParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编号");

            migrationBuilder.AlterColumn<int>(
                name: "WarningLife",
                table: "CutterModels",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "预警寿命");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数9");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数8");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数7");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数6");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数5");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数4");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数3");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数2");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数10");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数1");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterModels",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "初始寿命");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "名称");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterModels",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具类型Id");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNoPrefix",
                table: "CutterModels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "刀具编号前缀");

            migrationBuilder.AlterColumn<int>(
                name: "UsedLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "已用寿命");

            migrationBuilder.AlterColumn<int>(
                name: "RestLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "剩余寿命");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter9",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数9");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter8",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数8");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter7",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数7");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter6",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数6");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter5",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数5");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter4",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数4");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter3",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数3");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter2",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数2");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter10",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数10");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter1",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "刀具参数1");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalLife",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "原始寿命");

            migrationBuilder.AlterColumn<long>(
                name: "OperatorUserId",
                table: "CutterLoadAndUnloadRecords",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "操作人");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OperatorTime",
                table: "CutterLoadAndUnloadRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "操作时间");

            migrationBuilder.AlterColumn<int>(
                name: "OperationType",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "0：卸刀，1：装刀");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTypeId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具类型");

            migrationBuilder.AlterColumn<int>(
                name: "CutterTValue",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "刀位（刀具T值）");

            migrationBuilder.AlterColumn<string>(
                name: "CutterNo",
                table: "CutterLoadAndUnloadRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "刀具编号");

            migrationBuilder.AlterColumn<int>(
                name: "CutterModelId",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "刀具型号");

            migrationBuilder.AlterColumn<int>(
                name: "CountingMethod",
                table: "CutterLoadAndUnloadRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "寿命计数方式（0：按次数，1：按时间）");

            migrationBuilder.AlterColumn<string>(
                name: "RenderHtml",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "渲染Dom节点");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "标记");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CustomFields",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "名称");

            migrationBuilder.AlterColumn<int>(
                name: "MaxLength",
                table: "CustomFields",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "最大长度");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRequired",
                table: "CustomFields",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否必需");

            migrationBuilder.AlterColumn<string>(
                name: "HtmlTemplate",
                table: "CustomFields",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "编辑节点");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayType",
                table: "CustomFields",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "显示类型");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "CustomFields",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "编号");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Crafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "工艺名称");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Crafts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "工艺备注");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Crafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "工艺代码");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessOrder",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工艺加工顺序");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessName",
                table: "CraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "工艺名称");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "工艺ID");

            migrationBuilder.AlterColumn<string>(
                name: "ProcessCode",
                table: "CraftProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "工艺代码");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLastProcess",
                table: "CraftProcesses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否是最后工序");

            migrationBuilder.AlterColumn<int>(
                name: "CraftId",
                table: "CraftProcesses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品工艺Id");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ChatMessages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "用户Id");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "ChatMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "租户Id");

            migrationBuilder.AlterColumn<long>(
                name: "TargetUserId",
                table: "ChatMessages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "目标用户Id");

            migrationBuilder.AlterColumn<int>(
                name: "TargetTenantId",
                table: "ChatMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "目标租户Id");

            migrationBuilder.AlterColumn<int>(
                name: "Side",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "发送方/接收方");

            migrationBuilder.AlterColumn<Guid>(
                name: "SharedMessageId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "唯一标识(GUID)");

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverReadState",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "接收方是否已读");

            migrationBuilder.AlterColumn<int>(
                name: "ReadState",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "是否已读");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldComment: "信息");

            migrationBuilder.AlterColumn<string>(
                name: "PrinterName",
                table: "CartonSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "打印机名称");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPackingCount",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "最大装箱数");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnpackingRedo",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否允许拆箱重装");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnpackingAfterPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否允许打印后拆箱重装");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否打印");

            migrationBuilder.AlterColumn<bool>(
                name: "IsGoodOnly",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "只能装合格件");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinalTest",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否需要终检");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoPrint",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否自动打印");

            migrationBuilder.AlterColumn<bool>(
                name: "HasToFlow",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "必经流程");

            migrationBuilder.AlterColumn<bool>(
                name: "ForbidRepeatPacking",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "禁止重复装箱");

            migrationBuilder.AlterColumn<bool>(
                name: "ForbidHopSequence",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "禁止跳序");

            migrationBuilder.AlterColumn<string>(
                name: "FlowIds",
                table: "CartonSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "必经流程Id");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组Id");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CartonSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "箱号规则Id");

            migrationBuilder.AlterColumn<bool>(
                name: "AutoCartonNo",
                table: "CartonSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "自动生成箱码");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CartonSerialNumbers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "CartonSerialNumbers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "序列号");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "CartonSerialNumbers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "日期 20230106");

            migrationBuilder.AlterColumn<int>(
                name: "RealPackingCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "实际包装数");

            migrationBuilder.AlterColumn<int>(
                name: "PrintLabelCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "打印次数");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPackingCount",
                table: "Cartons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "最大包装数");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceGroupId",
                table: "Cartons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备组Id");

            migrationBuilder.AlterColumn<string>(
                name: "CartonNo",
                table: "Cartons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "箱号");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CartonRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "名称");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CartonRules",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否启用");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CartonRuleDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "值");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "类型");

            migrationBuilder.AlterColumn<int>(
                name: "StartIndex",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "开始位");

            migrationBuilder.AlterColumn<int>(
                name: "SequenceNo",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "顺序号");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "CartonRuleDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "Length",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "长度");

            migrationBuilder.AlterColumn<int>(
                name: "ExpansionKey",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "扩展键");

            migrationBuilder.AlterColumn<int>(
                name: "EndIndex",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "结束位");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CartonRuleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "箱号规则Id");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftSolutionItemId",
                table: "CartonRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "具体班次Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDay",
                table: "CartonRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "班次日");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "CartonRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "工件二维码");

            migrationBuilder.AlterColumn<string>(
                name: "CartonNo",
                table: "CartonRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "箱号");

            migrationBuilder.AlterColumn<int>(
                name: "CartonId",
                table: "CartonRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "箱Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "Yield",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "产量");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户Id");

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "工件状态");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "人员班次名称");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "班次方案名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "班次日期");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "Capacities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备班次名称");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "循环倍率");

            migrationBuilder.AlterColumn<bool>(
                name: "Qualified",
                table: "Capacities",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "质量");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "Capacities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "程序名称");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Capacities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "产品名称");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Capacities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品Id");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousLinkId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "连接前面一笔记录的DmpId");

            migrationBuilder.AlterColumn<string>(
                name: "PlanName",
                table: "Capacities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "生产计划名称");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "生产计划Id");

            migrationBuilder.AlterColumn<int>(
                name: "PlanAmount",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "生产计划目标产量");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "零件码");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalCount",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "原始计数器");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工单Id");

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "Capacities",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "Mongo同步时间");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Capacities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "备注");

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "设备班次Id");

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "Capacities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "Capacities",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<bool>(
                name: "IsValid",
                table: "Capacities",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否有效记录");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLineOutputOffline",
                table: "Capacities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "产线产量是否下线");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLineOutput",
                table: "Capacities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "是否是产线产出");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Capacities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "Capacities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "加工时长(秒)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DmpId",
                table: "Capacities",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "记录DmpId，用于内部计算使用");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Capacities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "系统日历key");

            migrationBuilder.AlterColumn<decimal>(
                name: "AccumulateCount",
                table: "Capacities",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "累积计数器");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CalibratorCodes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "值");

            migrationBuilder.AlterColumn<int>(
                name: "Key",
                table: "CalibratorCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "键");

            migrationBuilder.AlterColumn<int>(
                name: "CartonRuleId",
                table: "CalibratorCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "包装规则Id");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Calendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "当前年第几周（每年1月1日作为第一周）");

            migrationBuilder.AlterColumn<string>(
                name: "YYYYWeek",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldComment: "格式化显示(年-周)：2017-01");

            migrationBuilder.AlterColumn<string>(
                name: "YYYYMM",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldComment: "格式化显示（年-月）：2017-01");

            migrationBuilder.AlterColumn<string>(
                name: "YYYYISOWeek",
                table: "Calendars",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldComment: "格式化显示(ISO标准，年-周)：2017-01");

            migrationBuilder.AlterColumn<byte>(
                name: "Weekday",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前周第几天：5");

            migrationBuilder.AlterColumn<byte>(
                name: "WeekOfYear",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前周第几天");

            migrationBuilder.AlterColumn<byte>(
                name: "WeekOfMonth",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前月第几周：3");

            migrationBuilder.AlterColumn<string>(
                name: "WeekDayName",
                table: "Calendars",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "星期几，英文：Wednesday");

            migrationBuilder.AlterColumn<string>(
                name: "QuarterName",
                table: "Calendars",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldComment: "季度名称，英文：First");

            migrationBuilder.AlterColumn<byte>(
                name: "Quarter",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "季度：2");

            migrationBuilder.AlterColumn<string>(
                name: "MonthYear",
                table: "Calendars",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldComment: "格式化显示（月缩写-年）Jan2017");

            migrationBuilder.AlterColumn<string>(
                name: "MonthName",
                table: "Calendars",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "月份名称，英文：January");

            migrationBuilder.AlterColumn<byte>(
                name: "Month",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "月份");

            migrationBuilder.AlterColumn<string>(
                name: "MMYYYY",
                table: "Calendars",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldComment: "格式化显示（月年）：012017");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前年最后一天：2017-12-31（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfQuarter",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前季度最后一天：2017-03-31（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastDayOfMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前月最后一天：2017/1/31（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWorkday",
                table: "Calendars",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否是工作日；1是");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWeekend",
                table: "Calendars",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否是礼拜日；1是");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHoliday",
                table: "Calendars",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "是否是假期；1是");

            migrationBuilder.AlterColumn<byte>(
                name: "ISOWeekOfYear",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前年第几周（ISO标准，每年1月4日所在周作为第一周）");

            migrationBuilder.AlterColumn<string>(
                name: "HolidayText",
                table: "Calendars",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "假期名称，此字段目前未使用。礼拜日，工作日，假期字段一般会在另外的工厂日历中");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前年第一天：2017-01-01（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfQuarter",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前季度第一天：2017-01-01（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfNextYear",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "下一个年第一天：2018-01-01（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfNextMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "下个月第一天：2017-02-01（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstDayOfMonth",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "当前月第一天：2017/1/1（日期类型，需要格式化）");

            migrationBuilder.AlterColumn<string>(
                name: "DaySuffix",
                table: "Calendars",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldComment: "英文后缀,th");

            migrationBuilder.AlterColumn<short>(
                name: "DayOfYear",
                table: "Calendars",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldComment: "当前年第几天");

            migrationBuilder.AlterColumn<byte>(
                name: "Day",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前月第几天");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Calendars",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "自然日");

            migrationBuilder.AlterColumn<byte>(
                name: "DOWInMonth",
                table: "Calendars",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "当前月第几周(每月第一天是第一周的第一天)");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Calendars",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "自然日数字格式");

            migrationBuilder.AlterColumn<string>(
                name: "TargetTable",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档源表表名称");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedTable",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档目标表名称");

            migrationBuilder.AlterColumn<string>(
                name: "ArchivedMessage",
                table: "ArchiveEntries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "归档结果信息");

            migrationBuilder.AlterColumn<string>(
                name: "ArchiveValue",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档列内容");

            migrationBuilder.AlterColumn<long>(
                name: "ArchiveTotalCount",
                table: "ArchiveEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "归档总计数量");

            migrationBuilder.AlterColumn<long>(
                name: "ArchiveCount",
                table: "ArchiveEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "归档最后一次数据数量");

            migrationBuilder.AlterColumn<string>(
                name: "ArchiveColumn",
                table: "ArchiveEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "归档列名称");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "租户Id");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "支付状态");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentPeriodType",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "支付时长 月/年");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentId",
                table: "AppSubscriptionPayments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "支付Id");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "AppSubscriptionPayments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "发票号");

            migrationBuilder.AlterColumn<int>(
                name: "Gateway",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "支付方式");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "版本Id");

            migrationBuilder.AlterColumn<int>(
                name: "DayCount",
                table: "AppSubscriptionPayments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "日数量");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "AppSubscriptionPayments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "数量");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "AppBinaryObjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "租户key");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Bytes",
                table: "AppBinaryObjects",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldComment: "字节数据，存储图片或文件等的字节数据");

            migrationBuilder.AlterColumn<int>(
                name: "UserShiftDetailId",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户班次Id");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "用户Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "开始时间");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_StaffShiftName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "人员班次名称");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_SolutionName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "班次方案名称");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShiftDetail_ShiftDay",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "班次日期");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftDetail_MachineShiftName",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "设备班次名称");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "Alarms",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "程序名称");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Alarms",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品Id");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工序Id");

            migrationBuilder.AlterColumn<string>(
                name: "PartNo",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "工件编号");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "工单Id");

            migrationBuilder.AlterColumn<string>(
                name: "MongoCreationTime",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "Mongo同步时间");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alarms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "报警内容");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                table: "Alarms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "备注，报警信息手动维护在该字段");

            migrationBuilder.AlterColumn<int>(
                name: "MachinesShiftDetailId",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "设备班次Id");

            migrationBuilder.AlterColumn<long>(
                name: "MachineId",
                table: "Alarms",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "MachineCode",
                table: "Alarms",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "设备编号");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Alarms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "Alarms",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "持续时长（秒）");

            migrationBuilder.AlterColumn<int>(
                name: "DateKey",
                table: "Alarms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "系统日历Key");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Alarms",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "报警编号");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "AlarmInfos",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true,
                oldComment: "报警原因");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AlarmInfos",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true,
                oldComment: "报警内容");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "AlarmInfos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "设备Id");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AlarmInfos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldComment: "报警编号");

            migrationBuilder.AlterColumn<int>(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "到期后 等待续费日期");

            migrationBuilder.AlterColumn<int>(
                name: "TrialDayCount",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "到期日");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "月度价格");

            migrationBuilder.AlterColumn<int>(
                name: "ExpiringEditionId",
                table: "AbpEditions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "到期版本Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "AnnualPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "年度价格");
        }
    }
}
