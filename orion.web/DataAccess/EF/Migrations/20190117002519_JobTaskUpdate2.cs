using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class JobTaskUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTasks_UsageStatus_UsageStatusId",
                table: "JobTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsageStatus",
                table: "UsageStatus");

            migrationBuilder.RenameTable(
                name: "UsageStatus",
                newName: "UsageStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsageStatuses",
                table: "UsageStatuses",
                column: "UsageStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTasks_UsageStatuses_UsageStatusId",
                table: "JobTasks",
                column: "UsageStatusId",
                principalTable: "UsageStatuses",
                principalColumn: "UsageStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTasks_UsageStatuses_UsageStatusId",
                table: "JobTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsageStatuses",
                table: "UsageStatuses");

            migrationBuilder.RenameTable(
                name: "UsageStatuses",
                newName: "UsageStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsageStatus",
                table: "UsageStatus",
                column: "UsageStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTasks_UsageStatus_UsageStatusId",
                table: "JobTasks",
                column: "UsageStatusId",
                principalTable: "UsageStatus",
                principalColumn: "UsageStatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
