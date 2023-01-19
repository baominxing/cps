using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wimi.BtlCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateABP80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpEditions_Editions_Id",
                table: "AbpEditions");

            migrationBuilder.AlterColumn<string>(
                name: "TargetNotifiers",
                table: "UserNotifications",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetNotifiers",
                table: "Notifications",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpEditions_Editions_Id",
                table: "AbpEditions",
                column: "Id",
                principalTable: "Editions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpEditions_Editions_Id",
                table: "AbpEditions");

            migrationBuilder.AlterColumn<string>(
                name: "TargetNotifiers",
                table: "UserNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetNotifiers",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpEditions_Editions_Id",
                table: "AbpEditions",
                column: "Id",
                principalTable: "Editions",
                principalColumn: "Id");
        }
    }
}
