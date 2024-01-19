namespace Flavorique_MVC.Models
{
    public class CreateRecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<CategoryViewModel> TagList { get; set; }
    }
}
