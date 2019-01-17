using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class JobTaskUpdate : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "JobTasks",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "LegacyCode",
                table: "JobTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageStatusId",
                table: "JobTasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobTasks_UsageStatusId",
                table: "JobTasks",
                column: "UsageStatusId");

            migrationBuilder.CreateTable(
              name: "UsageStatus",
              columns: table => new
              {
                  UsageStatusId = table.Column<int>(nullable: false)
                      .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                  Name = table.Column<string>(nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_UsageStatus", x => x.UsageStatusId);
              });

            var arr = new object[,] {
            { (int)JobTasks.UsageStatus.Unkown, JobTasks.UsageStatus.Unkown.ToString()},
            { (int)JobTasks.UsageStatus.Enabled, JobTasks.UsageStatus.Enabled.ToString()},
            { (int)JobTasks.UsageStatus.Disabled, JobTasks.UsageStatus.Disabled.ToString()}
            };
            migrationBuilder.InsertData("UsageStatus", new string[] { "UsageStatusId", "Name" }, arr);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTasks_UsageStatus_UsageStatusId",
                table: "JobTasks",
                column: "UsageStatusId",
                principalTable: "UsageStatus",
                principalColumn: "UsageStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTasks_UsageStatus_UsageStatusId",
                table: "JobTasks");

            migrationBuilder.DropTable(
                name: "UsageStatus");

            migrationBuilder.DropIndex(
                name: "IX_JobTasks_UsageStatusId",
                table: "JobTasks");

            migrationBuilder.DropColumn(
                name: "LegacyCode",
                table: "JobTasks");

            migrationBuilder.DropColumn(
                name: "UsageStatusId",
                table: "JobTasks");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "JobTasks",
                newName: "ShortName");
        }
    }
}
