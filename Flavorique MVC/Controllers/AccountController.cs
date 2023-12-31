﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Flavorique_MVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace Flavorique_MVC.Controllers
{
	public class AccountController : Controller
	{
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
		{
			IEnumerable<UserInfo> users = new List<UserInfo>();
			using (var client = new HttpClient())
			{
				using (var response = await client.GetAsync("https://localhost:7147/api/Account"))
				{
					string apiResponse = await response.Content.ReadAsStringAsync();
					users = JsonConvert.DeserializeObject<List<UserInfo>>(apiResponse);
				}
			}
			return View(users);
		}

        //GET
        public async Task<IActionResult> Edit(string id)
        {
			var user = await _userManager.FindByIdAsync(id);
            var userInfo = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };

            return View(userInfo);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserInfo model)
        {
            try
            {
                var id = model.Id;

                var profileModel = new APIProfile
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                };

                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"https://localhost:7147/api/Account/user/admin/{id}",
                        new StringContent(JsonConvert.SerializeObject(profileModel), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        // Handle validation errors from the API
                        var validationErrors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result);

                        foreach (var error in validationErrors)
                        {
                            foreach (var errorMessage in error.Value)
                            {
                                ModelState.AddModelError(error.Key, errorMessage);
                            }
                        }

                        return View(model);
                    }

                    // Handle other non-success status codes
                    ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(model);
            }
        }

        // POST
        public async Task<IActionResult> Delete(string? id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.DeleteAsync($"https://localhost:7147/api/Account/admin-delete/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View();
            }
        }

        // GET
        public async Task<IActionResult> Details(string? id) 
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null) {
                    return NotFound();
                }

                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                };

                IEnumerable<ShortRecipe> recipes = new List<ShortRecipe>();
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/user/{user.UserName}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        recipes = JsonConvert.DeserializeObject<List<ShortRecipe>>(apiResponse);
                    }
                }

                var model = new AccountDetailViewModel
                {
                    UserInfo = userInfo,
                    Recipes = recipes,
                };

                return View(model);
            } 
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);

                return RedirectToAction("Index");
            }
        }

        // POST
        public async Task<IActionResult> ToggleEmailConfirm(string? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.PostAsync($"https://localhost:7147/api/Account/toggle-confirm-email/{id}", null))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            } catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View();
            }
        }
    }
}
