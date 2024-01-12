﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Flavorique_MVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.Xml;

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

        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber)
		{
			IEnumerable<UserInfo> users = new List<UserInfo>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;

            using (var client = new HttpClient())
			{
				using (var response = await client.GetAsync($"https://localhost:7147/api/Account?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}"))
				{
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<UserInfo>(), pageIndex = 1, totalPages = 1, count = 0 });

                    users = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
			}
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["UserNameSortParm"] = sortOrder == "username" ? "usernameDesc" : "username";
            ViewData["EmailSortParm"] = sortOrder == "email" ? "emailDesc" : "email";
            ViewData["PhoneSortParm"] = sortOrder == "phone" ? "phoneDesc" : "phone";
            ViewData["EmailConfirmedSortParm"] = sortOrder == "eConfirmed" ? "eConfirmedDesc" : "eConfirmed";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            var paginatedList = new PaginatedList<UserInfo>(users.ToList(), count, pageIndex, 5);

            return View(paginatedList);
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

        public IActionResult Login()
        {
            return Redirect("https://localhost:7147/Identity/Account/Login?from=m");
            //return View();
        }

        // Commented since its causing too much hassle, easier to redirect to api Login
        /*
        [HttpPost]
        public async Task<IActionResult> Login(APILoginModel model) {

            bool responseCheck = false;

            using (var handler = new HttpClientHandler())
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                handler.CookieContainer = cookieContainer;

                using (var client = new HttpClient(handler))
                {
                    {
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                        bool.TryParse(await jsonContent.ReadAsStringAsync(), out responseCheck);

                        var response = await client.PostAsync("https://localhost:7147/api/Account/login",
                            jsonContent);

                        _logger.LogInformation(await jsonContent.ReadAsStringAsync());
                        _logger.LogCritical(await response.Content.ReadAsStringAsync());

                    }
                }
            }

            return View();
        }
        */

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
