using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class AddExpenses3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentName",
                table: "Expenses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentName",
                table: "Expenses");
        }
    }
}
