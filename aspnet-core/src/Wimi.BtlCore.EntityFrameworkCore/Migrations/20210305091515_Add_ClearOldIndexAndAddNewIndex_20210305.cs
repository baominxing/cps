using Microsoft.EntityFrameworkCore.Migrations;

namespace Wimi.BtlCore.Migrations
{
    public partial class Add_ClearOldIndexAndAddNewIndex_20210305 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

			#region 删除原有索引

			migrationBuilder.Sql(@"
--Alarms
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_5_277576027__K12')
						DROP INDEX [_dta_index_Alarms_5_277576027__K12] ON [dbo].[Alarms];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_5_277576027__K2_12')
						DROP INDEX [_dta_index_Alarms_5_277576027__K2_12] ON [dbo].[Alarms];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_9_277576027__K10_23')
						DROP INDEX [_dta_index_Alarms_9_277576027__K10_23] ON [dbo].[Alarms];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='Index_For_Alarm_SyncMongoData')
						DROP INDEX [Index_For_Alarm_SyncMongoData] ON [dbo].[Alarms];
GO

--Capacities
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_5_1077578877__K34_1_8_9_10')
						DROP INDEX [_dta_index_Capacities_5_1077578877__K34_1_8_9_10] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K12_26')
						DROP INDEX [_dta_index_Capacities_9_437576597__K12_26] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K16_K15_1_22_23')
						DROP INDEX [_dta_index_Capacities_9_437576597__K16_K15_1_22_23] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K2_4_10_22')
						DROP INDEX [_dta_index_Capacities_9_437576597__K2_4_10_22] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K2_K8_K1_9')
						DROP INDEX [_dta_index_Capacities_9_437576597__K2_K8_K1_9] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K9_K2_K8D_K1')
						DROP INDEX [_dta_index_Capacities_9_437576597__K9_K2_K8D_K1] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_1')
						DROP INDEX [Index_For_Capacity_SyncMongoData_1] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_2')
						DROP INDEX [Index_For_Capacity_SyncMongoData_2] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_3')
						DROP INDEX [Index_For_Capacity_SyncMongoData_3] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Kafka')
						DROP INDEX [Index_For_Kafka] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_合格统计')
						DROP INDEX [Index_For_合格统计] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='IX_Capacities_IsLineOutput')
						DROP INDEX [IX_Capacities_IsLineOutput] ON [dbo].[Capacities];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='IX_Capacity')
						DROP INDEX [IX_Capacity] ON [dbo].[Capacities];
GO

--ShiftCalendars
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('ShiftCalendars', N'U') and NAME='_dta_index_ShiftCalendars_5_1598628738__K2_3_6_10')
						DROP INDEX [_dta_index_ShiftCalendars_5_1598628738__K2_3_6_10] ON [dbo].[ShiftCalendars];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('ShiftCalendars', N'U') and NAME='_dta_index_ShiftCalendars_5_1598628738__K2_K10_K1_K6_K5_K3_K11')
						DROP INDEX [_dta_index_ShiftCalendars_5_1598628738__K2_K10_K1_K6_K5_K3_K11] ON [dbo].[ShiftCalendars];
GO

--States
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_5_1749581271__K21_K2_K4_6_7')
						DROP INDEX [_dta_index_States_5_1749581271__K21_K2_K4_6_7] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_5_242099903__K27_1_6_7')
						DROP INDEX [_dta_index_States_5_242099903__K27_1_6_7] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_7_1749581271__K12_K1_K21_K2_K4_6_7_18_20')
						DROP INDEX [_dta_index_States_7_1749581271__K12_K1_K21_K2_K4_6_7_18_20] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K23_26')
						DROP INDEX [_dta_index_States_9_1749581271__K23_26] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K4_1_2_6')
						DROP INDEX [_dta_index_States_9_1749581271__K4_1_2_6] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K5_K1_K4')
						DROP INDEX [_dta_index_States_9_1749581271__K5_K1_K4] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K7_K2_K6_K1')
						DROP INDEX [_dta_index_States_9_1749581271__K7_K2_K6_K1] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='Index_For_State_SyncMongoData')
						DROP INDEX [Index_For_State_SyncMongoData] ON [dbo].[States];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='Index_For_人员绩效报表')
						DROP INDEX [Index_For_人员绩效报表] ON [dbo].[States];
GO

--TraceCatalogs
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceCatalogs', N'U') and NAME='_dta_index_TraceCatalogs_5_222623836__K3_K2_K8')
						DROP INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8] ON [dbo].[TraceCatalogs];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceCatalogs', N'U') and NAME='_dta_index_TraceCatalogs_5_222623836__K3_K2_K8_1_4_5_6_7_9_11_12')
						DROP INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8_1_4_5_6_7_9_11_12] ON [dbo].[TraceCatalogs];
GO

--TraceFlowRecords
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceFlowRecords', N'U') and NAME='_dta_index_TraceFlowRecords_5_254623950__K2')
						DROP INDEX [_dta_index_TraceFlowRecords_5_254623950__K2] ON [dbo].[TraceFlowRecords];
GO
");
			#endregion

			#region 创建现有索引
			migrationBuilder.Sql(@"
--Alarms
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Alarms', N'U') AND name = 'Index_For_Alarms_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_Alarms_Mdc_Select] ON [dbo].[Alarms]
(
	[MachineId] ASC,
	[MachinesShiftDetailId] ASC
)
INCLUDE([Code],[Message],[StartTime],[MachineCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Alarms', N'U') AND name = 'Index_For_Alarms_Mongo_Sync')
CREATE NONCLUSTERED INDEX [Index_For_Alarms_Mongo_Sync] ON [dbo].[Alarms]
(
	[MongoCreationTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

--Capacities
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Kafka_Update')
CREATE NONCLUSTERED INDEX [Index_For_Capacities_Kafka_Update] ON [dbo].[Capacities]
(
	[DmpId] ASC
)
INCLUDE([Duration],[EndTime],[StartTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_Capacities_Mdc_Select] ON [dbo].[Capacities]
(
	[MachinesShiftDetailId] ASC,
	[MachineId] ASC,
	[PlanId] ASC,
	[Qualified] ASC,
	[ShiftDetail_ShiftDay] ASC
)
INCLUDE([Duration],[EndTime],[ShiftDetail_MachineShiftName],[ShiftDetail_SolutionName],[ShiftDetail_StaffShiftName],[StartTime],[Tag],[UserId],[Yield]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Mongo_Sync')
CREATE NONCLUSTERED INDEX [Index_For_Capacities_Mongo_Sync] ON [dbo].[Capacities]
(
	[IsLineOutput] ASC
)
INCLUDE([MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


--ShiftCalendars
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_ShiftCalendars_Mdc_Select] ON [dbo].[ShiftCalendars]
(
	[MachineShiftDetailId] ASC
)
INCLUDE([ShiftSolutionId],[ShiftItemId],[MachineId],[ShiftDay],[ShiftDayName],[ShiftWeekName],[ShiftMonthName],[ShiftYearName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_2')
CREATE NONCLUSTERED INDEX [Index_For_ShiftCalendars_Mdc_Select_2] ON [dbo].[ShiftCalendars]
(
	[ShiftDayName] ASC
)
INCLUDE([ShiftSolutionId],[ShiftItemId],[MachineShiftDetailId],[MachineId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_3')
CREATE NONCLUSTERED INDEX [Index_For_ShiftCalendars_Mdc_Select_3] ON [dbo].[ShiftCalendars]
(
	[ShiftDay] ASC
)
INCLUDE([ShiftSolutionId],[ShiftItemId],[MachineShiftDetailId],[MachineId],[ShiftDayName],[ShiftMonth],[ShiftMonthName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_4')
CREATE NONCLUSTERED INDEX [Index_For_ShiftCalendars_Mdc_Select_4] ON [dbo].[ShiftCalendars]
(
	[ShiftSolutionId] ASC
)
INCLUDE([ShiftItemId],[MachineShiftDetailId],[MachineId],[ShiftYearName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_View')
CREATE NONCLUSTERED INDEX [Index_For_ShiftCalendars_View] ON [dbo].[ShiftCalendars]
(
	[ShiftSolutionId] ASC,
	[ShiftItemId] ASC,
	[MachineId] ASC
)
INCLUDE([ShiftItemSeq],[MachineShiftDetailId],[StartTime],[EndTime],[Duration],[ShiftDay],[ShiftDayName],[ShiftWeek],[ShiftWeekName],[ShiftMonth],[ShiftMonthName],[ShiftYear],[ShiftYearName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

--States
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Kafka_Update')
CREATE NONCLUSTERED INDEX [Index_For_States_Kafka_Update] ON [dbo].[States]
(
	[DmpId] ASC
)
INCLUDE([Duration],[EndTime],[StartTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_States_Mdc_Select] ON [dbo].[States]
(
	[MachineId] ASC,
	[MachinesShiftDetailId] ASC,
	[ShiftDetail_ShiftDay] ASC,
	[UserId] ASC
)
INCLUDE([Code],[DateKey],[Duration],[EndTime],[Name],[ShiftDetail_MachineShiftName],[ShiftDetail_SolutionName],[StartTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Mongo_Sync')
CREATE NONCLUSTERED INDEX [Index_For_States_Mongo_Sync] ON [dbo].[States]
(
	[IsShiftSplit] ASC
)
INCLUDE([MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

--TraceCatalogs
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('TraceCatalogs', N'U') AND name = 'Index_For_TraceCatalogs_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_TraceCatalogs_Mdc_Select] ON [dbo].[TraceCatalogs]
(
	[MachineShiftDetailId] ASC,
	[OnlineTime] ASC,
	[PartNo] ASC,
	[DeviceGroupId] ASC
)
INCLUDE([OfflineTime],[Qualified],[Id],[IsReworkPart],[PlanId],[ArchivedTable]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

--TraceFlowRecords
IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('TraceFlowRecords', N'U') AND name = 'Index_For_TraceFlowRecords_Mdc_Select')
CREATE NONCLUSTERED INDEX [Index_For_TraceFlowRecords_Mdc_Select] ON [dbo].[TraceFlowRecords]
(
	[PartNo] ASC
)
INCLUDE([MachineId],[Station],[EntryTime],[LeftTime],[Tag]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
");
			#endregion
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {
			#region 回滚原有索引

			migrationBuilder.Sql(@"
--Alarms
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_5_277576027__K12')
CREATE NONCLUSTERED INDEX [_dta_index_Alarms_5_277576027__K12] ON [dbo].[Alarms]
(
	[MachinesShiftDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_5_277576027__K2_12')
CREATE NONCLUSTERED INDEX [_dta_index_Alarms_5_277576027__K2_12] ON [dbo].[Alarms]
(
	[MachineId] ASC
)
INCLUDE([MachinesShiftDetailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='_dta_index_Alarms_9_277576027__K10_23')
CREATE NONCLUSTERED INDEX [_dta_index_Alarms_9_277576027__K10_23] ON [dbo].[Alarms]
(
	[UserId] ASC
)
INCLUDE([MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Alarms', N'U') and NAME='Index_For_Alarm_SyncMongoData')
CREATE NONCLUSTERED INDEX [Index_For_Alarm_SyncMongoData] ON [dbo].[Alarms]
(
	[MachineId] ASC,
	[StartTime] DESC
)
INCLUDE([Id],[MachineCode],[Code],[Message],[EndTime],[Duration],[Memo],[UserId],[UserShiftDetailId],[MachinesShiftDetailId],[OrderId],[ProcessId],[PartNo],[DateKey],[ProgramName],[ShiftDetail_SolutionName],[ShiftDetail_StaffShiftName],[ShiftDetail_MachineShiftName],[ShiftDetail_ShiftDay],[ProductId],[MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

--Capacities
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_5_1077578877__K34_1_8_9_10')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_5_1077578877__K34_1_8_9_10] ON [dbo].[Capacities]
(
	[DmpId] ASC
)
INCLUDE([Id],[StartTime],[EndTime],[Duration]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K12_26')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K12_26] ON [dbo].[Capacities]
(
	[IsValid] ASC
)
INCLUDE([MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K16_K15_1_22_23')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K16_K15_1_22_23] ON [dbo].[Capacities]
(
	[MachinesShiftDetailId] ASC,
	[UserShiftDetailId] ASC
)
INCLUDE([Id],[ShiftDetail_ShiftDay],[ShiftDetail_SolutionName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K2_4_10_22')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K2_4_10_22] ON [dbo].[Capacities]
(
	[MachineId] ASC
)
INCLUDE([Yield],[Duration],[ShiftDetail_ShiftDay]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K2_K8_K1_9')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K2_K8_K1_9] ON [dbo].[Capacities]
(
	[MachineId] ASC,
	[StartTime] ASC,
	[Id] ASC
)
INCLUDE([EndTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='_dta_index_Capacities_9_437576597__K9_K2_K8D_K1')
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K9_K2_K8D_K1] ON [dbo].[Capacities]
(
	[EndTime] ASC,
	[MachineId] ASC,
	[StartTime] DESC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_1')
CREATE NONCLUSTERED INDEX [Index_For_Capacity_SyncMongoData_1] ON [dbo].[Capacities]
(
	[MachinesShiftDetailId] ASC,
	[MachineId] ASC,
	[Id] ASC,
	[Tag] ASC,
	[IsLineOutput] ASC,
	[IsLineOutputOffline] ASC
)
INCLUDE([Yield]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_2')
CREATE NONCLUSTERED INDEX [Index_For_Capacity_SyncMongoData_2] ON [dbo].[Capacities]
(
	[Id] ASC,
	[DmpId] ASC,
	[EndTime] ASC,
	[PreviousLinkId] ASC,
	[StartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Capacity_SyncMongoData_3')
CREATE NONCLUSTERED INDEX [Index_For_Capacity_SyncMongoData_3] ON [dbo].[Capacities]
(
	[EndTime] ASC,
	[Id] ASC,
	[DmpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_Kafka')
CREATE NONCLUSTERED INDEX [Index_For_Kafka] ON [dbo].[Capacities]
(
	[DmpId] ASC
)
INCLUDE([Id],[StartTime],[EndTime],[Duration]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='Index_For_合格统计')
CREATE NONCLUSTERED INDEX [Index_For_合格统计] ON [dbo].[Capacities]
(
	[MachineId] ASC,
	[ShiftDetail_ShiftDay] ASC
)
INCLUDE([MachinesShiftDetailId],[IsValid],[Tag],[StartTime],[EndTime],[PartNo],[Qualified],[IsLineOutputOffline],[IsLineOutput],[Yield],[UserId],[ShiftDetail_SolutionName],[ShiftDetail_StaffShiftName],[ShiftDetail_MachineShiftName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='IX_Capacities_IsLineOutput')
CREATE NONCLUSTERED INDEX [IX_Capacities_IsLineOutput] ON [dbo].[Capacities]
(
	[IsLineOutput] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('Capacities', N'U') and NAME='IX_Capacity')
CREATE NONCLUSTERED INDEX [IX_Capacity] ON [dbo].[Capacities]
(
	[ShiftDetail_ShiftDay] ASC,
	[MachineId] ASC,
	[StartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO


--ShiftCalendars
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('ShiftCalendars', N'U') and NAME='_dta_index_ShiftCalendars_5_1598628738__K2_3_6_10')
CREATE NONCLUSTERED INDEX [_dta_index_ShiftCalendars_5_1598628738__K2_3_6_10] ON [dbo].[ShiftCalendars]
(
	[ShiftSolutionId] ASC
)
INCLUDE([ShiftItemId],[MachineId],[ShiftDay]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('ShiftCalendars', N'U') and NAME='_dta_index_ShiftCalendars_5_1598628738__K2_K10_K1_K6_K5_K3_K11')
CREATE NONCLUSTERED INDEX [_dta_index_ShiftCalendars_5_1598628738__K2_K10_K1_K6_K5_K3_K11] ON [dbo].[ShiftCalendars]
(
	[ShiftSolutionId] ASC,
	[ShiftDay] ASC,
	[Id] ASC,
	[MachineId] ASC,
	[MachineShiftDetailId] ASC,
	[ShiftItemId] ASC,
	[ShiftDayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

--States
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_5_1749581271__K21_K2_K4_6_7')
CREATE NONCLUSTERED INDEX [_dta_index_States_5_1749581271__K21_K2_K4_6_7] ON [dbo].[States]
(
	[ShiftDetail_ShiftDay] ASC,
	[MachineId] ASC,
	[Code] ASC
)
INCLUDE([StartTime],[EndTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_5_242099903__K27_1_6_7')
CREATE NONCLUSTERED INDEX [_dta_index_States_5_242099903__K27_1_6_7] ON [dbo].[States]
(
	[DmpId] ASC
)
INCLUDE([Id],[StartTime],[EndTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_7_1749581271__K12_K1_K21_K2_K4_6_7_18_20')
CREATE NONCLUSTERED INDEX [_dta_index_States_7_1749581271__K12_K1_K21_K2_K4_6_7_18_20] ON [dbo].[States]
(
	[MachinesShiftDetailId] ASC,
	[Id] ASC,
	[ShiftDetail_ShiftDay] ASC,
	[MachineId] ASC,
	[Code] ASC
)
INCLUDE([StartTime],[EndTime],[ShiftDetail_SolutionName],[ShiftDetail_MachineShiftName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K23_26')
CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K23_26] ON [dbo].[States]
(
	[IsShiftSplit] ASC
)
INCLUDE([MongoCreationTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K4_1_2_6')
CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K4_1_2_6] ON [dbo].[States]
(
	[Code] ASC
)
INCLUDE([Id],[MachineId],[StartTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K5_K1_K4')
CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K5_K1_K4] ON [dbo].[States]
(
	[Name] ASC,
	[Id] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='_dta_index_States_9_1749581271__K7_K2_K6_K1')
CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K7_K2_K6_K1] ON [dbo].[States]
(
	[EndTime] ASC,
	[MachineId] ASC,
	[StartTime] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='Index_For_State_SyncMongoData')
CREATE NONCLUSTERED INDEX [Index_For_State_SyncMongoData] ON [dbo].[States]
(
	[PreviousLinkId] ASC,
	[DmpId] ASC,
	[EndTime] ASC,
	[Id] ASC
)
INCLUDE([Duration],[StartTime]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('States', N'U') and NAME='Index_For_人员绩效报表')
CREATE NONCLUSTERED INDEX [Index_For_人员绩效报表] ON [dbo].[States]
(
	[ShiftDetail_ShiftDay] ASC
)
INCLUDE([MachineId],[Code],[Name],[StartTime],[EndTime],[Duration],[UserId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

--TraceCatalogs
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceCatalogs', N'U') and NAME='_dta_index_TraceCatalogs_5_222623836__K3_K2_K8')
CREATE NONCLUSTERED INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8] ON [dbo].[TraceCatalogs]
(
	[OnlineTime] ASC,
	[PartNo] ASC,
	[MachineShiftDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceCatalogs', N'U') and NAME='_dta_index_TraceCatalogs_5_222623836__K3_K2_K8_1_4_5_6_7_9_11_12')
CREATE NONCLUSTERED INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8_1_4_5_6_7_9_11_12] ON [dbo].[TraceCatalogs]
(
	[OnlineTime] ASC,
	[PartNo] ASC,
	[MachineShiftDetailId] ASC
)
INCLUDE([Id],[OfflineTime],[DeviceGroupId],[Qualified],[IsReworkPart],[PlanId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

--TraceFlowRecords
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('TraceFlowRecords', N'U') and NAME='_dta_index_TraceFlowRecords_5_254623950__K2')
CREATE NONCLUSTERED INDEX [_dta_index_TraceFlowRecords_5_254623950__K2] ON [dbo].[TraceFlowRecords]
(
	[PartNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
");
			#endregion

			#region 删除现有索引
			migrationBuilder.Sql(@"
--Alarms
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Alarms', N'U') AND name = 'Index_For_Alarms_Mdc_Select')
DROP INDEX [Index_For_Alarms_Mdc_Select] ON [dbo].[Alarms]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Alarms', N'U') AND name = 'Index_For_Alarms_Mongo_Sync')
DROP INDEX [Index_For_Alarms_Mongo_Sync] ON [dbo].[Alarms]
GO

--Capacities
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Kafka_Update')
DROP INDEX [Index_For_Capacities_Kafka_Update] ON [dbo].[Capacities]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Mdc_Select')
DROP INDEX [Index_For_Capacities_Mdc_Select] ON [dbo].[Capacities]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Capacities', N'U') AND name = 'Index_For_Capacities_Mongo_Sync')
DROP INDEX [Index_For_Capacities_Mongo_Sync] ON [dbo].[Capacities]
GO


--ShiftCalendars
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select')
DROP INDEX [Index_For_ShiftCalendars_Mdc_Select] ON [dbo].[ShiftCalendars]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_2')
DROP INDEX [Index_For_ShiftCalendars_Mdc_Select_2] ON [dbo].[ShiftCalendars]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_3')
DROP INDEX [Index_For_ShiftCalendars_Mdc_Select_2] ON [dbo].[ShiftCalendars]
GO


IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_Mdc_Select_4')
DROP INDEX [Index_For_ShiftCalendars_Mdc_Select_4] ON [dbo].[ShiftCalendars]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('ShiftCalendars', N'U') AND name = 'Index_For_ShiftCalendars_View')
DROP INDEX [Index_For_ShiftCalendars_View] ON [dbo].[ShiftCalendars]
GO

--States
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Kafka_Update')
DROP INDEX [Index_For_States_Kafka_Update] ON [dbo].[States]
GO

IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Mdc_Select')
DROP INDEX [Index_For_States_Mdc_Select] ON [dbo].[States]
GO


IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('States', N'U') AND name = 'Index_For_States_Mongo_Sync')
DROP INDEX [Index_For_States_Mongo_Sync] ON [dbo].[States]
GO

--TraceCatalogs
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('TraceCatalogs', N'U') AND name = 'Index_For_TraceCatalogs_Mdc_Select')
DROP INDEX [Index_For_TraceCatalogs_Mdc_Select] ON [dbo].[TraceCatalogs]
GO

--TraceFlowRecords
IF EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('TraceFlowRecords', N'U') AND name = 'Index_For_TraceFlowRecords_Mdc_Select')
DROP INDEX [Index_For_TraceFlowRecords_Mdc_Select] ON [dbo].[TraceFlowRecords]
GO
");
			#endregion
		}
    }
}
