﻿namespace Flavorique_Web_App.Models
{
    public class RecipeTag
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int TagId { get; set; }
        public Recipe Recipe { get; set; }
        public Tag Tag { get; set; }
    }
}
