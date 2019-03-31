using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class AddJobStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobStatusId",
                table: "Jobs",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "JobStatuses",
                columns: table => new
                {
                    JobStatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatuses", x => x.JobStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobStatusId",
                table: "Jobs",
                column: "JobStatusId");

            var arr = new object[,] {
            { (int)Jobs.JobStatus.Unkown, Jobs.JobStatus.Unkown.ToString()},
            { (int)Jobs.JobStatus.Enabled, Jobs.JobStatus.Enabled.ToString()},
            { (int)Jobs.JobStatus.Archived, Jobs.JobStatus.Archived.ToString()}
            };
            migrationBuilder.InsertData("JobStatuses", new string[] { "JobStatusId", "Name" }, arr);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobStatuses_JobStatusId",
                table: "Jobs",
                column: "JobStatusId",
                principalTable: "JobStatuses",
                principalColumn: "JobStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobStatuses_JobStatusId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobStatusId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobStatusId",
                table: "Jobs");
        }
    }
}
