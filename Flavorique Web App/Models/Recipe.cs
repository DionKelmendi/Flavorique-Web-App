using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Flavorique_Web_App.Models
{
    public class Recipe
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
        [Required]
        public string? Body { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }
    }
}
