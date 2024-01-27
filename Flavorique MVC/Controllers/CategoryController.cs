using Flavorique_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Flavorique_MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            IEnumerable<Category> categories = new List<Category>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Category>(), pageIndex = 1, totalPages = 1, count = 0 });

                    categories = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
            }

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "nameDesc" : "name";
            ViewData["DisplaySortParm"] = sortOrder == "display" ? "displayDesc" : "display";
            ViewData["DateSortParm"] = sortOrder == "date" ? "dateDesc" : "date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            _logger.LogWarning(categories.Count().ToString());

            var paginatedList = new PaginatedList<Category>(categories.ToList(), count, pageIndex, 5);

            return View(paginatedList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            try
            {

				IEnumerable<CategoryDetailViewModel> responseObject = new List<CategoryDetailViewModel>();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Category/tags"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            _logger.LogWarning(apiResponse);
                            responseObject = JsonConvert.DeserializeObject<IEnumerable<CategoryDetailViewModel>>(apiResponse);
                        }
                    }
                }

                foreach (var obj in responseObject) {
                    _logger.LogWarning(obj.Category.Name);

                    foreach (var tag in obj.Tags)
                    {
						_logger.LogWarning(tag.Name);
					}
				}

                var model = responseObject.Where(r => r.Category.Id == id).FirstOrDefault();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Index");
            }
        }


        //GET
        public async Task<IActionResult> Create()
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("https://localhost:7147/api/Category",
                        new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, category);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(category);
            }
        }

        //GET
        public async Task<IActionResult> Edit(int? id)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = new Category();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category/{id}"))
                {
                    if (response.IsSuccessStatusCode) {
                        string result = await response.Content.ReadAsStringAsync();
                        category = JsonConvert.DeserializeObject<Category>(result);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(category);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                var id = category.Id;

                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"https://localhost:7147/api/Category/{id}",
                        new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, category);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(category);
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response, Category category)
        {
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

                return View(category);
            }

            // Handle other non-success status codes
            ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
            return View(category);
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            try {

                if (id == null || id == 0)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.DeleteAsync($"https://localhost:7147/api/Category/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View();
            }
        }

        public async Task<string> GetUserRole()
        {
            var role = "User";

            using (var handler = new HttpClientHandler())
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                handler.CookieContainer = cookieContainer;

                using (var client = new HttpClient(handler))
                {
                    {
                        var response = await client.GetAsync("https://localhost:7147/api/Account/user/role");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        role = apiResponse;
                    }
                }
            }
            return role;
        }
    }
}
