using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class correctedListingContractRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Contracts_ContractID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_ContractID",
                table: "Listings");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractListing");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ContractID",
                table: "Listings",
                column: "ContractID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Contracts_ContractID",
                table: "Listings",
                column: "ContractID",
                principalTable: "Contracts",
                principalColumn: "ContractID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
