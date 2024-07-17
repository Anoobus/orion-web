using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class AddInternalOnlyCatDesignation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInternalOnly",
                table: "TaskCategories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternalOnly",
                table: "TaskCategories");
        }
    }
}
