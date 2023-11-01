using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class addedCustomerRelationshipContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Customers_CustomerID",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_CustomerID",
                table: "Listings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Listings");

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CustomerID",
                table: "Contracts",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ListingID",
                table: "Contracts",
                column: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Customer_CustomerID",
                table: "Contracts",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Customer_CustomerID",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_CustomerID",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ListingID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                columns: new[] { "ListingID", "AgentID" });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_CustomerID",
                table: "Listings",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Customers_CustomerID",
                table: "Listings",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
