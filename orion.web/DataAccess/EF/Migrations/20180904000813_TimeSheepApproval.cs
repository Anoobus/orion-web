using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class TimeSheepApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeSheetApprovals",
                columns: table => new
                {
                    TimeSheetApprovalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    ApproverEmployeeId = table.Column<int>(nullable: true),
                    TimeApprovalStatus = table.Column<string>(nullable: true),
                    ApprovalDate = table.Column<DateTime>(nullable: true),
                    ResponseReason = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    WeekId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheetApprovals", x => x.TimeSheetApprovalId);
                    table.ForeignKey(
                        name: "FK_TimeSheetApprovals_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetApprovals_EmployeeId",
                table: "TimeSheetApprovals",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeSheetApprovals");
        }
    }
}
