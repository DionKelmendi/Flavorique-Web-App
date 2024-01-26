namespace Flavorique_MVC.Models
{
    public class AccountDetailViewModel
    {
        public UserInfo UserInfo { get; set; }
        public IEnumerable<ShortRecipe> Recipes { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
