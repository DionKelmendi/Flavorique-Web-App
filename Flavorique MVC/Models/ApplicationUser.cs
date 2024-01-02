﻿using Microsoft.AspNetCore.Identity;

namespace Flavorique_MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
