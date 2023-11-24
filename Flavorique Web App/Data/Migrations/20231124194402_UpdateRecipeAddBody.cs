using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flavorique_Web_App.Data.Migrations
{
    public partial class UpdateRecipeAddBody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Recipes",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Recipes",
                newName: "Name");
        }
    }
}
