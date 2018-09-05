using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class DroppedCategoryFromJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_TaskCategories_TaskCategoryId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_TaskCategoryId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TaskCategoryId",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskCategoryId",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TaskCategoryId",
                table: "Jobs",
                column: "TaskCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_TaskCategories_TaskCategoryId",
                table: "Jobs",
                column: "TaskCategoryId",
                principalTable: "TaskCategories",
                principalColumn: "TaskCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
