using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace orion.web.DataAccessEFMigrations
{
    public partial class AddExpenseTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArcFlashlabelExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    DateOfInvoice = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalLabelsCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPostageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArcFlashlabelExpenditures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArcFlashlabelExpenditures_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractorExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrionPONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPOContractAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorExpenditures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorExpenditures_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiscExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscExpenditures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiscExpenditures_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAndExpenceExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAndExpenceExpenditures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeAndExpenceExpenditures_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyVehicleExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    DateVehicleFirstUsed = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompanyVehicleId = table.Column<int>(type: "int", nullable: false),
                    TotalNumberOfDaysUsed = table.Column<int>(type: "int", nullable: false),
                    TotalMiles = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyVehicleExpenditures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyVehicleExpenditures_CompanyVehicles_CompanyVehicleId",
                        column: x => x.CompanyVehicleId,
                        principalTable: "CompanyVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyVehicleExpenditures_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArcFlashlabelExpenditures_JobId",
                table: "ArcFlashlabelExpenditures",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVehicleExpenditures_CompanyVehicleId",
                table: "CompanyVehicleExpenditures",
                column: "CompanyVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVehicleExpenditures_JobId",
                table: "CompanyVehicleExpenditures",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorExpenditures_JobId",
                table: "ContractorExpenditures",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_MiscExpenditures_JobId",
                table: "MiscExpenditures",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAndExpenceExpenditures_JobId",
                table: "TimeAndExpenceExpenditures",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArcFlashlabelExpenditures");

            migrationBuilder.DropTable(
                name: "CompanyVehicleExpenditures");

            migrationBuilder.DropTable(
                name: "ContractorExpenditures");

            migrationBuilder.DropTable(
                name: "MiscExpenditures");

            migrationBuilder.DropTable(
                name: "TimeAndExpenceExpenditures");

            migrationBuilder.DropTable(
                name: "CompanyVehicles");
        }
    }
}
