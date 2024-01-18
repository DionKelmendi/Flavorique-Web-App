namespace Flavorique_MVC.Models
{
    public class DetailRecipeViewModel
    {
        public Recipe Recipe{ get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
