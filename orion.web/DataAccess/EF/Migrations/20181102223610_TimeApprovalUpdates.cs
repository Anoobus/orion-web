using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class TimeApprovalUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "TimeSheetApprovals",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOverTimeHours",
                table: "TimeSheetApprovals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRegularHours",
                table: "TimeSheetApprovals",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "TimeSheetApprovals");

            migrationBuilder.DropColumn(
                name: "TotalOverTimeHours",
                table: "TimeSheetApprovals");

            migrationBuilder.DropColumn(
                name: "TotalRegularHours",
                table: "TimeSheetApprovals");
        }
    }
}
