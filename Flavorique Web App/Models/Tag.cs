using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace Flavorique_Web_App.Models
{
    public class Tag
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        public string Name { get; set; }
    }
}
