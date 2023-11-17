using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace Flavorique_Web_App.Models
{
    public class Recipe
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        // Foreign key
        //public int UserId { get; set; }

        // Navigation property
       // public User Author { get; set; }

       
    }
}
