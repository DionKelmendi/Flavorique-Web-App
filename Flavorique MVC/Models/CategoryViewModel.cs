namespace Flavorique_MVC.Models
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
