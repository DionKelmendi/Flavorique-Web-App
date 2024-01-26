namespace Flavorique_MVC.Models
{
    public class DetailTagViewModel
    {
        public Tag Tag { get; set; }
        public IEnumerable<ShortRecipe> Recipes { get; set; }
    }
}
