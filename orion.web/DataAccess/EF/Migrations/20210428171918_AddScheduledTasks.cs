using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class AddScheduledTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTasks",
                columns: table => new
                {
                    ScheduleTaskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    RecurEveryNWeeks = table.Column<int>(nullable: false),
                    OnMonday = table.Column<bool>(nullable: false),
                    OnTuesday = table.Column<bool>(nullable: false),
                    OnWednesday = table.Column<bool>(nullable: false),
                    OnThursday = table.Column<bool>(nullable: false),
                    OnFriday = table.Column<bool>(nullable: false),
                    OnSaturday = table.Column<bool>(nullable: false),
                    OnSunday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTasks", x => x.ScheduleTaskId);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTaskRunLogs",
                columns: table => new
                {
                    ScheduleTaskRunLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleTaskId = table.Column<int>(nullable: false),
                    TaskCompletionDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTaskRunLogs", x => x.ScheduleTaskRunLogId);
                    table.ForeignKey(
                        name: "FK_ScheduleTaskRunLogs_ScheduleTasks_ScheduleTaskId",
                        column: x => x.ScheduleTaskId,
                        principalTable: "ScheduleTasks",
                        principalColumn: "ScheduleTaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTaskRunLogs_ScheduleTaskId",
                table: "ScheduleTaskRunLogs",
                column: "ScheduleTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTaskRunLogs");

            migrationBuilder.DropTable(
                name: "ScheduleTasks");
        }
    }
}
