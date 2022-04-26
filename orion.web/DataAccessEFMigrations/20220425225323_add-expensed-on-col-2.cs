using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace orion.web.DataAccessEFMigrations
{
    public partial class addexpensedoncol2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpensedOn",
                table: "ContractorExpenditures",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpensedOn",
                table: "ContractorExpenditures");
        }
    }
}
