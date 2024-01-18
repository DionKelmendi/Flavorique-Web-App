using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flavorique_MVC.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Body { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    public DateTime UpdatedDateTime { get; set; }
    public string? AuthorId { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}
