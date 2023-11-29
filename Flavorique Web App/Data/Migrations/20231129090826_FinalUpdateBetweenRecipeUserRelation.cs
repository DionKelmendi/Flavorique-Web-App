using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flavorique_Web_App.Data.Migrations
{
    public partial class FinalUpdateBetweenRecipeUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Recipes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_AspNetUsers_AuthorId",
                table: "Recipes",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_AspNetUsers_AuthorId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Recipes");
        }
    }
}
