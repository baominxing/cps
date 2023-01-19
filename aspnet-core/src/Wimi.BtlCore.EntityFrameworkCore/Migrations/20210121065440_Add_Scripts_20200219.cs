using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using Wimi.BtlCore.Migrations.Helpers;

namespace Wimi.BtlCore.Migrations
{
    public partial class Add_Scripts_20200219 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var basePath = MigrateHelper.GetSqlScriptBasePath();

            if (string.IsNullOrEmpty(basePath)) return;

            migrationBuilder.Sql(@"
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'IF'
          AND name = 'func_GetStateSummeryByDate'
)
    DROP FUNCTION dbo.func_GetStateSummeryByDate;
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'IF'
          AND name = 'func_SplitInts'
)
    DROP FUNCTION dbo.func_SplitInts;
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_InitCalendar'
)
    DROP PROC [dbo].[sp_InitCalendar];
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_CurrentStateDurationStatistics'
)
    DROP PROC [dbo].[sp_CurrentStateDurationStatistics];
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_GetPreviousMachineShiftDetailList'
)
    DROP PROC [dbo].[sp_GetPreviousMachineShiftDetailList];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_RefillWorkWhenSyncCompleted'
)
    DROP PROC [dbo].[sp_RefillWorkWhenSyncCompleted];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_StateDurationStatistics'
)
    DROP PROC [dbo].[sp_CurrentStateDurationStatistics];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'TR'
          AND name = 'Trigger_TraceFlowSettings_Insert'
)
    DROP TRIGGER [dbo].[Trigger_TraceFlowSettings_Insert];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'V'
          AND name = 'ShiftCalendarsView'
)
    DROP VIEW [dbo].[ShiftCalendarsView];
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Capacity')
    CREATE INDEX IX_Capacity
    ON Capacities (
                      ShiftDetail_ShiftDay,
                      MachineId,
                      StartTime
                  );
GO
");
            //create functions
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "functions", "dbo.func_SplitInts.UserDefinedFunction.sql"));
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "functions", "dbo.func_GetStateSummeryByDate.UserDefinedFunction.sql"));
            //create procedures            
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_CurrentStateDurationStatistics.StoredProcedure.sql"));
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_GetPreviousMachineShiftDetailList.StoredProcedure.sql"));
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_InitCalendar.StoredProcedure.sql"));
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_RefillWorkWhenSyncCompleted.StoredProcedure.sql"));
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_StateDurationStatistics.StoredProcedure.sql"));

            //create triggers
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "triggers", "Trigger_TraceFlowSettings_Insert.sql"));
            //create views
            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "views", "ShiftCalendarsView.sql"));

            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_ListMachineStates.sql"));

            //init Calendar
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM Calendars)
BEGIN
    DECLARE @StartDate DATETIME;
    SET @StartDate = GETDATE();
    EXEC sp_InitCalendar @StartDate, @NumberOfYears = 15;
END;
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'IF'
          AND name = 'func_GetStateSummeryByDate'
)
    DROP FUNCTION dbo.func_GetStateSummeryByDate;
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'IF'
          AND name = 'func_SplitInts'
)
    DROP FUNCTION dbo.func_SplitInts;
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_InitCalendar'
)
    DROP PROC [dbo].[sp_InitCalendar];
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_CurrentStateDurationStatistics'
)
    DROP PROC [dbo].[sp_CurrentStateDurationStatistics];
GO

IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_GetPreviousMachineShiftDetailList'
)
    DROP PROC [dbo].[sp_GetPreviousMachineShiftDetailList];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_RefillWorkWhenSyncCompleted'
)
    DROP PROC [dbo].[sp_RefillWorkWhenSyncCompleted];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_StateDurationStatistics'
)
    DROP PROC [dbo].[sp_CurrentStateDurationStatistics];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'TR'
          AND name = 'Trigger_TraceFlowSettings_Insert'
)
    DROP TRIGGER [dbo].[Trigger_TraceFlowSettings_Insert];
GO
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'V'
          AND name = 'ShiftCalendarsView'
)
    DROP VIEW [dbo].[ShiftCalendarsView];
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Capacity')
    DROP INDEX IX_Capacity ON Capacities;
GO
");

            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sysobjects WHERE type='P' AND name='sp_ListMachineStates')
	                    DROP PROC dbo.sp_ListMachineStates;
                  GO");
        }
    }
}
