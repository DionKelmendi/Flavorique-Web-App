namespace Flavorique_Web_App.DTOs
{
    public class CreateCommentDto
    {
        public string Body { get; set; }
        public int Rating { get; set; }
        public int RecipeId { get; set; }
    }
}
