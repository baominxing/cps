using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using Wimi.BtlCore.Migrations.Helpers;

namespace Wimi.BtlCore.Migrations
{
    public partial class Add_Procedures_GetArchiveTargetTableByPartNo_20210520 : Migration
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
    WHERE type = 'P'
          AND name = 'sp_GetArchiveTargetTableByPartNo'
)
    DROP PROCEDURE dbo.sp_GetArchiveTargetTableByPartNo;
GO
");

            migrationBuilder.SqlFile(Path.Combine(basePath, "SQLScripts", "procedures", "dbo.sp_GetArchiveTargetTableByPartNo.sql"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS
(
    SELECT 1
    FROM sysobjects
    WHERE type = 'P'
          AND name = 'sp_GetArchiveTargetTableByPartNo'
)
    DROP PROCEDURE dbo.sp_GetArchiveTargetTableByPartNo;
GO
");
        }
    }
}
