using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wimi.BtlCore.Migrations
{
    public partial class Add_ArchiveEntity_20210203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchiveEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchivedTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchiveColumn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchiveValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchiveCount = table.Column<long>(type: "bigint", nullable: false),
                    ArchiveTotalCount = table.Column<long>(type: "bigint", nullable: false),
                    ArchivedMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveEntries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchiveEntries");
        }
    }
}
