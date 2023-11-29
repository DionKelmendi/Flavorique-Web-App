using Microsoft.AspNetCore.Identity;

namespace Flavorique_Web_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
