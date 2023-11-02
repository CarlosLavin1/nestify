using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class adjustedCustomerListingRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Agent_AgentID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Customer_CustomerID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Listing_ListingID",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ListingID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ListingID",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerID",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ContractID",
                table: "Listings",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_CustomerID",
                table: "Listings",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Agents_AgentID",
                table: "Contracts",
                column: "AgentID",
                principalTable: "Agents",
                principalColumn: "AgentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Customers_CustomerID",
                table: "Contracts",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Contracts_ContractID",
                table: "Listings",
                column: "ContractID",
                principalTable: "Contracts",
                principalColumn: "ContractID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Customers_CustomerID",
                table: "Listings",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Agents_AgentID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Customers_CustomerID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Contracts_ContractID",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Customers_CustomerID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_ContractID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_CustomerID",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Listings");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerID",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_Contract_Agent_AgentID",
                table: "Contracts",
                column: "AgentID",
                principalTable: "Agents",
                principalColumn: "AgentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Customer_CustomerID",
                table: "Contracts",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Listing_ListingID",
                table: "Contracts",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
