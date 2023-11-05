using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class addedImageRelationshipsToListingAndAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_AgentId",
                table: "Image",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_ListingId",
                table: "Image",
                column: "ListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Agents_AgentId",
                table: "Image",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Listings_ListingId",
                table: "Image",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "ListingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Agents_AgentId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Listings_ListingId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_AgentId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_ListingId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ListingId",
                table: "Image");
        }
    }
}
