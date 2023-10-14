using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcNestify.Data.Migrations
{
    public partial class AddedEmpDateToAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfEmployment",
                table: "Agents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfEmployment",
                table: "Agents");
        }
    }
}
