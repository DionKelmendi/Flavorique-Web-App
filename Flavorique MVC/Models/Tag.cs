using Flavorique_MVC.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace Flavorique_MVC.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}


