using System.ComponentModel.DataAnnotations;

namespace Flavorique_Web_App.Models
{
	public class UserInfo
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }
	}
}
