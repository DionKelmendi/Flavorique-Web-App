using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Flavorique_Web_App.Models;
using Flavorique_Web_App.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Flavorique_Web_App.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> _logger;
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly RazorViewToStringRenderer _razorViewToStringRenderer;

		public AccountController(ILogger<AccountController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, RazorViewToStringRenderer razorViewToStringRenderer)
		{
			_logger = logger;
			_db = db;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_razorViewToStringRenderer = razorViewToStringRenderer;
		}

		[HttpGet]
		public IActionResult GetUsers()
		{
			var users = _userManager.Users
				.Select(user => new UserInfo
				{
					   UserId = user.Id,
					   Email = user.Email,
					   UserName = user.UserName,
					   PhoneNumber = user.PhoneNumber,
					   TwoFactorEnabled = user.TwoFactorEnabled
				})
				.ToList();

			return Ok(users);
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
				UserId = user.Id,
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
				UserId = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				EmailConfirmed = user.EmailConfirmed,
				TwoFactorEnabled = user.TwoFactorEnabled,
			};

			return Ok(userInfo);
		}

		/*
		[HttpGet("email/{email}")]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var userInfo = new UserInfo
			{
				UserId = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
				EmailConfirmed = user.EmailConfirmed,
				TwoFactorEnabled = user.TwoFactorEnabled,
			};

			return Ok(userInfo);
		}
		*/

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
				UserId = user.Id,
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

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] APILoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				return BadRequest("Invalid email or password");
			}

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				// Successful login
				return Ok("Login successful");
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
	}
}
