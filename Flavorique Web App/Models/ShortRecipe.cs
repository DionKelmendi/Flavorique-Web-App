namespace Flavorique_Web_App.Models
{
    public class ShortRecipe
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
    }
}
