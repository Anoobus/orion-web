using Microsoft.EntityFrameworkCore.Migrations;
using Orion.Web.Api.Expenditures.Models;
using Orion.Web.Employees;

#nullable disable

namespace Orion.Web.DataAccessEFMigrations
{
    public partial class SupervisorConcept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {       
            migrationBuilder.CreateTable(
                name: "EmployeeDirectReport",
                columns: table => new
                {
                    EmployeeDirectReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupervisorEmployeeId = table.Column<int>(type: "int", nullable: false),
                    ReportEmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDirectReport", x => x.EmployeeDirectReportId);
                    table.ForeignKey(
                        name: "FK_EmployeeDirectReport_Employees_SupervisorEmployeeId",
                        column: x => x.SupervisorEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDirectReport_SupervisorEmployeeId_ReportEmployeeId",
                table: "EmployeeDirectReport",
                columns: new[] { "SupervisorEmployeeId", "ReportEmployeeId" },
                unique: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId", 
                keyValue: 3,
                column:  "Name", 
                value: UserRoleName.Supervisor);
            
            migrationBuilder.UpdateData(
                table: "CompanyVehicles",
                keyColumn: "Id", 
                keyValue: 1,
                column:  "Name", 
                value: CompanyVehicleDescriptor.ChevyBlazer.ToString());
            
            migrationBuilder.UpdateData(
                table: "CompanyVehicles",
                keyColumn: "Id", 
                keyValue: 2,
                column:  "Name", 
                value: CompanyVehicleDescriptor.GMCTruck.ToString());
            
            migrationBuilder.UpdateData(
                table: "CompanyVehicles",
                keyColumn: "Id", 
                keyValue: 3,
                column:  "Name", 
                value: CompanyVehicleDescriptor.ChevyTruck.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeDirectReport");

            migrationBuilder.UpdateData(table: "UserRoles",
                        keyColumn: "UserRoleId", 
                        keyValue: 3,
                        column:  "Name", 
                        value: "Limited");
        }
    }
}
