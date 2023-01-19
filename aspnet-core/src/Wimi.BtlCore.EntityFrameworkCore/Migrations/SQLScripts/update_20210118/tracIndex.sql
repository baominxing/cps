SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_TraceFlowRecords_5_254623950__K2] ON [dbo].[TraceFlowRecords]
(
	[PartNo] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8_1_4_5_6_7_9_11_12] ON [dbo].[TraceCatalogs]
(
	[OnlineTime] ASC,
	[PartNo] ASC,
	[MachineShiftDetailId] ASC
)
INCLUDE ( 	[Id],
	[OfflineTime],
	[DeviceGroupId],
	[Qualified],
	[IsReworkPart],
	[PlanId]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_TraceCatalogs_5_222623836__K3_K2_K8] ON [dbo].[TraceCatalogs]
(
	[OnlineTime] ASC,
	[PartNo] ASC,
	[MachineShiftDetailId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


CREATE STATISTICS [_dta_stat_222623836_8_2_3] ON [dbo].[TraceCatalogs]([MachineShiftDetailId], [PartNo], [OnlineTime])

