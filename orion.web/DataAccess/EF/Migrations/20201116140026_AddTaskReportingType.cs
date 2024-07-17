using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class AddTaskReportingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportingClassificationId",
                table: "JobTasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"CREATE VIEW [dbo].[vEmployeeeWeeklySummary]
                                    AS
                                    SELECT EmployeeId, SUM(ISNULL(Hours, 0)) AS TotalRegularHours, SUM(ISNULL(OvertimeHours, 0)) AS TotalOverTimeHours, WeekId
                                    FROM     dbo.TimeEntries
                                    GROUP BY EmployeeId, WeekId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportingClassificationId",
                table: "JobTasks");

            migrationBuilder.Sql(@"DROP VIEW [dbo].[vEmployeeeWeeklySummary]");
        }
    }
}
