namespace Flavorique_MVC.Models
{
    public class EditRecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<CategoryViewModel> TagList { get; set; }
        public string TagString { get; set; }
    }

}
