namespace Flavorique_MVC.Models
{
    public class HomeViewModel
    {
        public int? UserCount { get; set; }
        public int? CategoryCount { get; set; }
        public int? TagCount { get; set; }
        public int? RecipeCount { get; set; }
        public string? AverageRating { get; set; }
        public HomeRatingGraphData? GraphData { get; set; }
        public IEnumerable<ShortRecipe> MostRecentRecipes { get; set; }
        public IEnumerable<TagGraphItem> TagGraphItems { get; set; }
        public TagGraphItem MostUsedTag { get; set; }
    }
}
