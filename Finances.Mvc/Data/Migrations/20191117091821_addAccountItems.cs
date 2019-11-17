using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Finances.Mvc.Data.Migrations
{
    public partial class addAccountItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountCode = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    BankCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    InputDate = table.Column<DateTime>(nullable: true),
                    MandateId = table.Column<string>(nullable: true),
                    PartnerName = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ValueDate = table.Column<DateTime>(nullable: true),
                    ConnectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountItems_ConnectionData_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "ConnectionData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountItems_ConnectionId",
                table: "AccountItems",
                column: "ConnectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountItems");
        }
    }
}
