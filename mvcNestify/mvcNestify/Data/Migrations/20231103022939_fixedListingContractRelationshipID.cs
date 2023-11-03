using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class fixedListingContractRelationshipID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractListing");

            migrationBuilder.AddColumn<int>(
                name: "ListingID",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ListingID",
                table: "Contracts",
                column: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Listings_ListingID",
                table: "Contracts",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Listings_ListingID",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ListingID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ListingID",
                table: "Contracts");

            migrationBuilder.CreateTable(
                name: "ContractListing",
                columns: table => new
                {
                    ContractID = table.Column<int>(type: "int", nullable: false),
                    ListingsListingID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractListing", x => new { x.ContractID, x.ListingsListingID });
                    table.ForeignKey(
                        name: "FK_ContractListing_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractListing_Listings_ListingsListingID",
                        column: x => x.ListingsListingID,
                        principalTable: "Listings",
                        principalColumn: "ListingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractListing_ListingsListingID",
                table: "ContractListing",
                column: "ListingsListingID");
        }
    }
}
