namespace Flavorique_Web_App.Models
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
