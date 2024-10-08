﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Orion.Web.DataAccess.EF.Migrations
{
    public partial class AddEmpExtId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalEmployeeId",
                table: "Employees",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalEmployeeId",
                table: "Employees");
        }
    }
}
