using Microsoft.AspNetCore.Mvc;
using Flavorique_MVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_MVC.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;

        public TagController(ILogger<TagController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber, string categoryFilter)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            IEnumerable<Tag> tags = new List<Tag>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Tag?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}&category={categoryFilter}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Tag>(), pageIndex = 1, totalPages = 1, count = 0 });

                    tags = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
            }

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "nameDesc" : "name";
            ViewData["CategorySortParm"] = sortOrder == "category" ? "categoryDesc" : "category";
            ViewData["CategoryFilterParm"] = categoryFilter;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            _logger.LogWarning(tags.Count().ToString());

            var paginatedList = new PaginatedList<Tag>(tags.ToList(), count, pageIndex, 5);

            IEnumerable<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Category>() });

                    categories = responseObject.data;
                }
            }

            var model = new TagIndexViewModel{ 
                PaginatedList = paginatedList,
                Categories = categories
            };

            return View(model);
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
                var tag = new Tag();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Tag/{id}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            tag = JsonConvert.DeserializeObject<Tag>(result);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }

                IEnumerable<ShortRecipe> recipes = new List<ShortRecipe>();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/tag/{id}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            recipes = JsonConvert.DeserializeObject<List<ShortRecipe>>(apiResponse);
                        }
                    }
                }

                var model = new DetailTagViewModel
                {
                    Tag = tag,
                    Recipes = recipes
                };

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

            IEnumerable<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Category>()});

                    categories = responseObject.data;
                }
            }

            var model = new CreateTagViewModel
            {
                Tag = new Tag(),
                Categories = categories
            };

            return View(model);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("https://localhost:7147/api/Tag",
                        new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, tag);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(tag);
            }
        }

        // GET
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

            var tag = new Tag();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Tag/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        tag = JsonConvert.DeserializeObject<Tag>(result);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            var model = new CreateTagViewModel
            {
                Tag = tag,
                Categories = await GetCategories()
            };

            return View(model);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag tag)
        {
            try
            {
                var id = tag.Id;
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"https://localhost:7147/api/Tag/{id}",
                        new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json"));
                    _logger.LogInformation("Handling Response");
                    return await HandleResponse(response, tag);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(tag);
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response, Tag tag)
        {
            _logger.LogInformation($"Response: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var result = await response.Content.ReadAsStringAsync();

            var model = new CreateTagViewModel
            {
                Tag = tag,
                Categories = await GetCategories()
            };

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

        private async Task<IEnumerable<Category>> GetCategories() {
            IEnumerable<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Category>() });

                    categories = responseObject.data;
                }
            }

            return categories;
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.DeleteAsync($"https://localhost:7147/api/Tag/{id}"))
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

        public async Task<string> GetUserRole() {

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
