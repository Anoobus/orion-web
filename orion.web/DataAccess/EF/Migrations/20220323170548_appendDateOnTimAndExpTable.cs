﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orion.Web.DataAccessEFMigrations
{
    public partial class AppendDateOnTimAndExpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpenseOnDate",
                table: "TimeAndExpenceExpenditures",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseOnDate",
                table: "TimeAndExpenceExpenditures");
        }
    }
}
