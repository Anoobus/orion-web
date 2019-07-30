using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class AddExpenses2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Expenses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Expenses");
        }
    }
}
