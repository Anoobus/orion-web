using Microsoft.EntityFrameworkCore.Migrations;

namespace orion.web.DataAccess.EF.Migrations
{
    public partial class RemoveClientCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientCode",
                table: "Clients");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientCode",
                table: "Clients",
                nullable: true);
        }
    }
}
