namespace Flavorique_MVC.Models
{
    public class CategoryDetailViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
