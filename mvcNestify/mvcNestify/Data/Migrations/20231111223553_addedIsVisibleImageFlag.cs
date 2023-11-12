using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class addedIsVisibleImageFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Image");
        }
    }
}
