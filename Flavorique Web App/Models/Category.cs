﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Flavorique_Web_App.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
