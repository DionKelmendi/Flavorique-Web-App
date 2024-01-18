using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flavorique_Web_App.Migrations
{
    public partial class RemovedCommentListFromRecipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Recipes_RecipeId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_RecipeId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RecipeId1",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecipeId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RecipeId1",
                table: "Comments",
                column: "RecipeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Recipes_RecipeId1",
                table: "Comments",
                column: "RecipeId1",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
