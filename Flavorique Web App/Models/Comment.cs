using System.ComponentModel.DataAnnotations;

namespace Flavorique_Web_App.Models;

public class Comment
{
    [Key]
    public int Id {  get; set; }
    [Required]
    public string Body { get; set; }
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    public DateTime UpdatedDateTime { get; set; }
    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}
