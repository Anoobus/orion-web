using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class EmployeeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Employees",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "First",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExempt",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Last",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "First",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsExempt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Last",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Employees",
                newName: "Name");
        }
    }
}
