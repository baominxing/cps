CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K7_K2_K6_K1] ON [dbo].[States]
(
	[EndTime] ASC,
	[MachineId] ASC,
	[StartTime] ASC,
	[Id] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]



SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K5_K1_K4] ON [dbo].[States]
(
	[Name] ASC,
	[Id] ASC,
	[Code] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


--1   done
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K4_1_2_6] ON [dbo].[States]
(
	[Code] ASC
)
INCLUDE ( 	[Id],
	[MachineId],
	[StartTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


--2  done
	CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K9_K2_K8D_K1] ON [dbo].[Capacities]
(
	[EndTime] ASC,
	[MachineId] ASC,
	[StartTime] DESC,
	[Id] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


--3  done
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K16_K15_1_22_23] ON [dbo].[Capacities]
(
	[MachinesShiftDetailId] ASC,
	[UserShiftDetailId] ASC
)
INCLUDE ( 	[Id],
	[ShiftDetail_ShiftDay],
	[ShiftDetail_SolutionName]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


--4  done
CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K2_K8_K1_9] ON [dbo].[Capacities]
(
	[MachineId] ASC,
	[StartTime] ASC,
	[Id] ASC
)
INCLUDE ( 	[EndTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]



SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_Alarms_9_277576027__K10_23] ON [dbo].[Alarms]
(
	[UserId] ASC
)
INCLUDE ( 	[MongoCreationTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]



SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K12_26] ON [dbo].[Capacities]
(
	[IsValid] ASC
)
INCLUDE ( 	[MongoCreationTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]




SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_States_9_1749581271__K23_26] ON [dbo].[States]
(
	[IsShiftSplit] ASC
)
INCLUDE ( 	[MongoCreationTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]



CREATE NONCLUSTERED INDEX [_dta_index_Capacities_9_437576597__K2_4_10_22] ON [dbo].[Capacities]
(
	[MachineId] ASC
)
INCLUDE ( 	[Yield],
	[Duration],
	[ShiftDetail_ShiftDay]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

---------------------------------------------------------------------------------------------------------

CREATE STATISTICS [_dta_stat_1749581271_1_21_2] ON [dbo].[States]([Id], [ShiftDetail_ShiftDay], [MachineId])



CREATE STATISTICS [_dta_stat_1749581271_2_12_21_4] ON [dbo].[States]([MachineId], [MachinesShiftDetailId], [ShiftDetail_ShiftDay], [Code])

CREATE STATISTICS [_dta_stat_1749581271_4_2_1_21_12] ON [dbo].[States]([Code], [MachineId], [Id], [ShiftDetail_ShiftDay], [MachinesShiftDetailId])

CREATE STATISTICS [_dta_stat_1749581271_2_1] ON [dbo].[States]([MachineId], [Id])

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_States_7_1749581271__K12_K1_K21_K2_K4_6_7_18_20] ON [dbo].[States]
(
	[MachinesShiftDetailId] ASC,
	[Id] ASC,
	[ShiftDetail_ShiftDay] ASC,
	[MachineId] ASC,
	[Code] ASC
)
INCLUDE ( 	[StartTime],
	[EndTime],
	[ShiftDetail_SolutionName],
	[ShiftDetail_MachineShiftName]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

