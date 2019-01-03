using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class RemoveYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "TimeSheetApprovals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "TimeSheetApprovals",
                nullable: false,
                defaultValue: 0);
        }
    }
}
