namespace Flavorique_MVC.Models
{
    public class TagIndexViewModel
    {
        public PaginatedList<Tag> PaginatedList { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
