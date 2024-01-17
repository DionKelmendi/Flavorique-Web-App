namespace Flavorique_MVC.Models
{
    public class CreateTagViewModel
    {
        public Tag Tag { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
