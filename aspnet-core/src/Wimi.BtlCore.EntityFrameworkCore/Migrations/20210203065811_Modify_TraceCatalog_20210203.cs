using Microsoft.EntityFrameworkCore.Migrations;

namespace Wimi.BtlCore.Migrations
{
    public partial class Modify_TraceCatalog_20210203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivedTable",
                table: "TraceFlowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedTable",
                table: "TraceCatalogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedTable",
                table: "TraceFlowRecords");

            migrationBuilder.DropColumn(
                name: "ArchivedTable",
                table: "TraceCatalogs");
        }
    }
}
