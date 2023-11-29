﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
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
        //[Required]
        //public string Ingredients { get; set; }
        //[Required]
        //public string Instructions { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }

}
}
