using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Flavorique_MVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;

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

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
            }
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["TitleSortParm"] = sortOrder == "title" ? "titleDesc" : "title";
            ViewData["DateSortParm"] = sortOrder == "date" ? "dateDesc" : "date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            _logger.LogWarning(recipes.Count().ToString());

            var paginatedList = new PaginatedList<ShortRecipe>(recipes.ToList(), count, pageIndex, 5);

            return View(paginatedList);
        }

        //GET
        public async Task<IActionResult> Details(int? id)
        {
            var recipe = new Recipe();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recipe  = JsonConvert.DeserializeObject<Recipe>(apiResponse);
                }
            }
            return View(recipe);
        }

        //GET
        public IActionResult Create()
        {
            if (userSignedIn())
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe)
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

                        return await HandleResponse(response, recipe);
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

            if (!userSignedIn())
            {
                return RedirectToAction("Index");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            _logger.LogInformation("Starting recipe fetch");

            var recipe = new Recipe();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recipe = JsonConvert.DeserializeObject<Recipe>(apiResponse);
                    _logger.LogInformation(apiResponse);

                }
            }

            _logger.LogWarning("Ended recipe fetch");

            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe recipe)
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

                        return await HandleResponse(response, recipe);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(recipe);
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response, Recipe recipe)
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

                return View(recipe);
            }

            // Handle other non-success status codes
            ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
            return View(recipe);
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
    }
}
