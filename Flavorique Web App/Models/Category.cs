using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Flavorique_Web_App.Models
{
    
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    
    }
}
