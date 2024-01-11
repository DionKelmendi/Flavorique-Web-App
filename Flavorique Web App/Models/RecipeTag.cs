using System.ComponentModel.DataAnnotations;

namespace Flavorique_Web_App.Models;

public class RecipeTag
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int RecipeId { get; set; }
    [Required]
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    public Recipe Recipe { get; set; }
}
