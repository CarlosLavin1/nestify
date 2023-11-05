using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class addedShowingAgentRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentID",
                table: "Showings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Showings_AgentID",
                table: "Showings",
                column: "AgentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Showings_Agents_AgentID",
                table: "Showings",
                column: "AgentID",
                principalTable: "Agents",
                principalColumn: "AgentID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showings_Agents_AgentID",
                table: "Showings");

            migrationBuilder.DropIndex(
                name: "IX_Showings_AgentID",
                table: "Showings");

            migrationBuilder.DropColumn(
                name: "AgentID",
                table: "Showings");
        }
    }
}
