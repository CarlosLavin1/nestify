using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class addedSalePriceToListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesPrice",
                table: "Contracts");

            migrationBuilder.AddColumn<decimal>(
                name: "SalesPrice",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesPrice",
                table: "Listings");

            migrationBuilder.AddColumn<decimal>(
                name: "SalesPrice",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
