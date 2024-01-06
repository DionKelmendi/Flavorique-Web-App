using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Flavorique_Web_App.Models
{
	public class APIProfile
	{
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Phone]
		[Display(Name = "Phone number")]
		public string? PhoneNumber { get; set; }
	}
}
