using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Flavorique_Web_App.Models;
using Flavorique_Web_App.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Flavorique_Web_App.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly RazorViewToStringRenderer _razorViewToStringRenderer;

		public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, RazorViewToStringRenderer razorViewToStringRenderer)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_razorViewToStringRenderer = razorViewToStringRenderer;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers(string? sortOrder, string? searchString, int? pageNumber)
		{
			IEnumerable<UserInfo> users = _userManager.Users
				.Select(user => new UserInfo
				{
					   Id = user.Id,
					   Email = user.Email,
					   UserName = user.UserName,
					   PhoneNumber = user.PhoneNumber,
					   EmailConfirmed = user.EmailConfirmed,
					   TwoFactorEnabled = user.TwoFactorEnabled
				})
				.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.ToLower().Contains(searchString.ToLower()) || u.Email.ToLower().Contains(searchString.ToLower()));
            }
            int count = users.Count();

            switch (sortOrder)
            {
                case "username":
                    users = users.OrderBy(u => u.UserName);
                    break;
                case "usernameDesc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                case "email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "emailDesc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case "phone":
                    users = users.OrderBy(u => u.PhoneNumber);
                    break;
                case "phoneDesc":
                    users = users.OrderByDescending(u => u.PhoneNumber);
                    break;
                case "eConfirmed":
                    users = users.OrderByDescending(u => u.EmailConfirmed);
                    break;
                case "eConfirmedDesc":
                    users = users.OrderBy(u => u.EmailConfirmed);
                    break;
                case "idDesc":
                    users = users.OrderByDescending(u => u.Id);
                    break;
                default:
                    users = users.OrderBy(u => u.Id);
                    break;
            }

            int pageSize = 5;
            PaginatedList<UserInfo> result = await PaginatedList<UserInfo>.CreateAsync(users, pageNumber ?? 1, pageSize);
            return Ok(new { data = result, pageIndex = result.PageIndex, totalPages = result.TotalPages, count = count });
		}

		[HttpGet("user")]
		public async Task<IActionResult> GetUser()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound("User not found");
			}

            var userInfo = new UserInfo
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				EmailConfirmed = user.EmailConfirmed,
				TwoFactorEnabled = user.TwoFactorEnabled,
			};

			return Ok(userInfo);
		}

		[HttpGet("id/{id}")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var userInfo = new UserInfo
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				EmailConfirmed = user.EmailConfirmed,
				TwoFactorEnabled = user.TwoFactorEnabled,
			};

			return Ok(userInfo);
		}

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetUserByName(string name)
		{
			var user = await _userManager.FindByNameAsync(name);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var userInfo = new UserInfo
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				EmailConfirmed = user.EmailConfirmed,
				TwoFactorEnabled = user.TwoFactorEnabled,
			};

			return Ok(userInfo);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] APIRegisterModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					UserName = model.UserName,
					Email = model.Email,
				};

				var existingUser = await _userManager.FindByEmailAsync(model.Email);

				if (existingUser != null) {
					return BadRequest("Email is already taken.");
				}

				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code }, // , returnUrl = returnUrl
						protocol: Request.Scheme);

					var viewName = "~/Views/Email/ConfirmEmail.cshtml";
					var emailModel = new ConfirmEmailViewModel
					{
						Url = callbackUrl,
						UserName = model.Email
					};

					var htmlString = await _razorViewToStringRenderer.RenderViewToStringAsync(viewName, emailModel);

					await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
					htmlString);

					return Ok("User registered successfully");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return BadRequest(ModelState);
		}


		// Currently unused
		// Works fine in API, but I cant figure out how to create the Cookie code that Identity Uses, so using the default login page
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] APILoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				return BadRequest("Invalid email or password");
			}

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            
			var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;

            foreach (var claim in claims)
			{
                _logger.LogCritical(claim.ToString());
            }

            if (result.Succeeded)
			{
                // Successful login
                return Ok(true);
            }

			if (result.RequiresTwoFactor)
			{
				// Handle two-factor authentication if needed
				return BadRequest("Two-factor authentication is required");
			}

			if (result.IsLockedOut)
			{
				// Handle account lockout if needed
				return BadRequest("Account is locked out");
			}

			// Failed login
			return BadRequest("Invalid email or password");
		}

		[HttpGet("isUserSignedIn")]
		public IActionResult isUserSignedIn()
		{
			var signedIn = Request.Cookies[".AspNetCore.Identity.Application"];
            return Ok(signedIn != null);
		}

		[HttpPost("logout")]
        public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return Ok("Logout successful");
		}

		[HttpPost("resend-email/{email}")]
		public async Task<IActionResult> ResendEmailConfirmation(string email) 
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return BadRequest("User not found");
			}

			if (user.EmailConfirmed) {
				return BadRequest("Email is already confirmed.");
			}

			var userId = user.Id;
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { area = "Identity", userId = userId, code = code }, // , returnUrl = returnUrl
				protocol: Request.Scheme);

			var viewName = "~/Views/Email/ConfirmEmail.cshtml";
			var emailModel = new ConfirmEmailViewModel
			{
				Url = callbackUrl,
				UserName = email
			};

			var htmlString = await _razorViewToStringRenderer.RenderViewToStringAsync(viewName, emailModel);

			await _emailSender.SendEmailAsync(email, "Confirm your email",
			htmlString);

			return Ok("Email sent. Check your inbox.");
		}

		[HttpPost("forgot-password/{email}")]
		public async Task<IActionResult> ForgotPassword(string email) 
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return BadRequest("User not found");
			}

			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ResetPassword",
				pageHandler: null,
				values: new { area = "Identity", code },
				protocol: Request.Scheme);

			var viewName = "~/Views/Email/ForgotPassword.cshtml";
			var emailModel = new ConfirmEmailViewModel
			{
				Url = callbackUrl,
				UserName = email
			};

			var htmlString = await _razorViewToStringRenderer.RenderViewToStringAsync(viewName, emailModel);

			await _emailSender.SendEmailAsync(email, "Confirm your email",
			htmlString);
			return Ok("Email sent. Check your inbox.");
		}

		// Just for testing only
		// Remove after creating the necessary Views
		[HttpPost("toggle-confirm-email/{id}")]
		public async Task<IActionResult> ToggleConfirmEmail(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("User not found");
			}

            user.EmailConfirmed = !user.EmailConfirmed;

            var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				return Ok("Email confirmed set to " + user.EmailConfirmed);
			}

			// Handle errors if the update fails
			return BadRequest("Failed to toggle confirm email");
		}

		[HttpPost("change-password/{id}")]
		public async Task<IActionResult> ChangePassword([FromBody] APIChangePasswordModel model, string id) 
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return Ok("Password changed successfully");
			}

			return BadRequest("Failed to change password");
		}

		[HttpDelete("user-delete")]
		public async Task<IActionResult> DeleteAccount(string password)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var passwordCheck = await _userManager.CheckPasswordAsync(user, password);

			if (!passwordCheck)
			{
				return BadRequest("Invalid password");
			}

			var result = await _userManager.DeleteAsync(user);

			if (result.Succeeded)
			{
				await _signInManager.SignOutAsync();

				return Ok("Account deleted successfully");
			}
			
			return BadRequest("Failed to delete account");
		}

		[HttpDelete("admin-delete/{id}")]
		public async Task<IActionResult> AdminDeleteAccount(string id)
		{

			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var result = await _userManager.DeleteAsync(user);

			if (result.Succeeded)
			{
				await _signInManager.SignOutAsync();

				return Ok("Account deleted successfully");
			}

			return BadRequest("Failed to delete account");
		}

        [HttpPut("user/{id}")]
        public async Task<IActionResult> PutAccount(APIProfile model, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Email != model.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email is already in use.");
                }
            }

            if (user.UserName != model.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);

                if (existingUser != null)
                {
                    return BadRequest("Username is already in use.");
                }
            }

			var confirmedEmail = user.EmailConfirmed;

            var emailResult = await _userManager.SetEmailAsync(user, model.Email);
            var userNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

			_logger.LogWarning(emailResult.Succeeded.ToString());
			_logger.LogWarning(userNameResult.Succeeded.ToString());
			_logger.LogWarning(phoneResult.Succeeded.ToString());

            if (emailResult.Succeeded && userNameResult.Succeeded && phoneResult.Succeeded)
            {
                user.EmailConfirmed = confirmedEmail;
                var result = await _userManager.UpdateAsync(user);

                return Ok("Information changed successfully");
            }

            return BadRequest("Failed to change info.");
        }
    }
}
