using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Flavorique_MVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.Data.SqlClient;

namespace Flavorique_MVC.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RecipeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RecipeController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            IEnumerable<ShortRecipe> recipes = new List<ShortRecipe>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<ShortRecipe>(), pageIndex = 1, totalPages = 1, count = 0 });

                    recipes = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;
                }
            }
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["TitleSortParm"] = sortOrder == "title" ? "titleDesc" : "title";
            ViewData["DateSortParm"] = sortOrder == "date" ? "dateDesc" : "date";
            ViewData["RatingSortParm"] = sortOrder == "rating" ? "ratingDesc" : "rating";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            var paginatedList = new PaginatedList<ShortRecipe>(recipes.ToList(), count, pageIndex, 5);

            return View(paginatedList);
        }

        //GET
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var userRole = await GetUserRole();

                if (!userRole.Equals("Admin"))
                {
                    return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
                }

                var recipe = new Recipe();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        recipe = JsonConvert.DeserializeObject<Recipe>(apiResponse);
                    }
                }

                var comments = new List<Comment>();
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Comment/GetCommentsByRecipe/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (!apiResponse.Contains("There are no comments"))
                        {
                            comments = JsonConvert.DeserializeObject<List<Comment>>(apiResponse);
                        }
                    }
                }

                var tags = new List<RecipeTag>();
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/RecipeTag?recipeId={id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (!apiResponse.Contains("There are no comments"))
                        {
                            tags = JsonConvert.DeserializeObject<List<RecipeTag>>(apiResponse);
                        }
                    }
                }

                var model = new DetailRecipeViewModel
                {
                    Recipe = recipe,
                    Tags = tags,
                    Comments = comments
                };
                return View(model);
            } catch (Exception ex) {
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

            if (userSignedIn())
            {
                IEnumerable<CategoryViewModel> tags = new List<CategoryViewModel>();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Category/tags"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<List<CategoryViewModel>>(apiResponse);

                        tags = responseObject;

                    }
                }

                var model = new CreateRecipeViewModel { 
                    Recipe = new Recipe(),
                    TagList = tags
                };

                return View(model);
            }
            return RedirectToAction("Index");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe, [FromForm] string selectedTags)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    var cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                    handler.CookieContainer = cookieContainer;

                    using (var client = new HttpClient(handler))
                    {
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(recipe), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("https://localhost:7147/api/Recipe", jsonContent);

                        return await HandleResponse(response, recipe, selectedTags, "create");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError("Body", $"An error occurred: {ex.Message}");
                return View(recipe);
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

            if (!userSignedIn())
            {
                return RedirectToAction("Index");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var recipe = new Recipe();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recipe = JsonConvert.DeserializeObject<Recipe>(apiResponse);

                }
            }

            if (recipe == null)
            {
                return NotFound();
            }

            IEnumerable<CategoryViewModel> tags = new List<CategoryViewModel>();
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Category/tags"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeObject<List<CategoryViewModel>>(apiResponse);

                    tags = responseObject;
                }
            }

            string tagString = string.Empty;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/RecipeTag/String?recipeId={id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    tagString = apiResponse;
                }
            }

            var model = new EditRecipeViewModel
            {
                Recipe = recipe,
                TagList = tags,
                TagString = tagString
            };
            return View(model);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe recipe, [FromForm] string selectedTags)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    var cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                    handler.CookieContainer = cookieContainer;

                    using (var client = new HttpClient(handler))
                    {
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(recipe), Encoding.UTF8, "application/json");
                        var response = await client.PutAsync($"https://localhost:7147/api/Recipe/{recipe.Id}", jsonContent);

                        return await HandleResponse(response, recipe, selectedTags, "edit");
                    }
                }
            }
            catch (Exception ex)
            {
                IEnumerable<CategoryViewModel> tags = new List<CategoryViewModel>();
                using (var client = new HttpClient())
                {
                    using (var tagListResponse = await client.GetAsync($"https://localhost:7147/api/Category/tags"))
                    {
                        string apiResponse = await tagListResponse.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<List<CategoryViewModel>>(apiResponse);
                        tags = responseObject;
                    }
                }

                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                var returnModel = new EditRecipeViewModel
                {
                    Recipe = recipe,
                    TagList = tags,
                    TagString = selectedTags
                };
                return View(returnModel);
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response, Recipe recipe, string? selectedTags, string requestType)
        {
            _logger.LogInformation("Starting handleResponse");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Response was successful {selectedTags}");

                var responseBody = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Response Body: {responseBody}");

                var responseObject = JsonConvert.DeserializeObject<Recipe>(responseBody);
                var recipeId = responseObject.Id;


                if (!string.IsNullOrEmpty(selectedTags))
                {
                    _logger.LogInformation("Starting selectedTags thingy");

                    var model = new FillRecipeTableModel { 
                        RecipeId = recipeId,
                        TagIds = selectedTags
                    };

                    using (var client = new HttpClient())
                    {
                        var recipeTagresponse = await client.PostAsync("https://localhost:7147/api/Recipe/RecipeTag/Update",
                            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                    }
                }

                return RedirectToAction("Index");
            }

            var result = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Response was NOT successful");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogCritical("Response was BadRequest");

                // Handle validation errors from the API
                var validationErrors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result);

                foreach (var error in validationErrors)
                {
                    foreach (var errorMessage in error.Value)
                    {
                        ModelState.AddModelError(error.Key, errorMessage);
                    }
                }

                IEnumerable<CategoryViewModel> tags = new List<CategoryViewModel>();
                using (var client = new HttpClient())
                {
                    using (var tagListResponse = await client.GetAsync($"https://localhost:7147/api/Category/tags"))
                    {
                        string apiResponse = await tagListResponse.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<List<CategoryViewModel>>(apiResponse);

                        tags = responseObject;
                    }
                }

                if (requestType == "edit")
                {
                    var returnModel = new EditRecipeViewModel
                    {
                        Recipe = recipe,
                        TagList = tags,
                        TagString = selectedTags
                    };
                    return View(returnModel);

                }
                if (requestType == "create")
                {
                    var returnModel = new CreateRecipeViewModel
                    {
                        Recipe = recipe,
                        TagList = tags
                    };
                    return View(returnModel);

                }

                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.DeleteAsync($"https://localhost:7147/api/Recipe/{id}"))
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

        // GET
        public async Task<IActionResult> Print(int? id)
        {
            var userRole = await GetUserRole();

            if (!userRole.Equals("Admin"))
            {
                return Redirect("https://localhost:7147/Identity/Account/Login?from=r");
            }

            try
            {
                var recipe = new PrintViewModel();

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/print?id={id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        recipe = JsonConvert.DeserializeObject<PrintViewModel>(apiResponse);
                    }
                }
                return View(recipe);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        private bool userSignedIn()
        {
            var signedIn = Request.Cookies[".AspNetCore.Identity.Application"];
            return signedIn != null;
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
