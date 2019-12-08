using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Finances.Mvc.Data.Migrations
{
    public partial class extend_connection_data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Balance",
                table: "ConnectionData",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSync",
                table: "ConnectionData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "ConnectionData");

            migrationBuilder.DropColumn(
                name: "LastSync",
                table: "ConnectionData");
        }
    }
}
