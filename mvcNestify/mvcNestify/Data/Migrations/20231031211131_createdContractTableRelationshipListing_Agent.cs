using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class createdContractTableRelationshipListing_Agent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Agents_AgentID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_AgentID",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "AgentID",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "SalesPrice",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Listings");

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    AgentID = table.Column<int>(type: "int", nullable: false),
                    ListingID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => new { x.ListingID, x.AgentID });
                    table.ForeignKey(
                        name: "FK_Contract_Agent_AgentID",
                        column: x => x.AgentID,
                        principalTable: "Agents",
                        principalColumn: "AgentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contract_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listings",
                        principalColumn: "ListingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AgentID",
                table: "Contracts",
                column: "AgentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "AgentID",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SalesPrice",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Listings_AgentID",
                table: "Listings",
                column: "AgentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Agents_AgentID",
                table: "Listings",
                column: "AgentID",
                principalTable: "Agents",
                principalColumn: "AgentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
