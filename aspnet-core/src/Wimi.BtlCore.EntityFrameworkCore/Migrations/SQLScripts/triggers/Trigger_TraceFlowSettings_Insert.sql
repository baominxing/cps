/****** Object:  Trigger [dbo].[Trigger_TraceFlowSettings_Insert]    Script Date: 2020/2/19 14:28:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Trigger_TraceFlowSettings_Insert] 
   ON  [dbo].[TraceFlowSettings]
   AFTER INSERT
AS 
BEGIN

	SET NOCOUNT ON;

    declare @QualityMakerFlowId int,@Id int

	select @Id = id, @QualityMakerFlowId = QualityMakerFlowId from inserted

	if @QualityMakerFlowId = 0
		Update TraceFlowSettings Set QualityMakerFlowId = @Id where Id = @Id
END
GO

ALTER TABLE [dbo].[TraceFlowSettings] ENABLE TRIGGER [Trigger_TraceFlowSettings_Insert]
GO

