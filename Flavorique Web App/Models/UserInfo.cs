namespace Flavorique_Web_App.Models
{
	public class UserInfo
	{
		public string UserId { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string PhoneNumber { get; set; }
		public bool EmailConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }
	}
}
