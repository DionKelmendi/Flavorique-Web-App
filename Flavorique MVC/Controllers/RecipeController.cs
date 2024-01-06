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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RecipeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Recipe> recipes = new List<Recipe>();
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost:7147/api/Recipe"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recipes = JsonConvert.DeserializeObject<List<Recipe>>(apiResponse);
                }
            }
            return View(recipes);
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
            if (_signInManager.IsSignedIn(User))
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
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }
                recipe.AuthorId = user.Id;

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("https://localhost:7147/api/Recipe",
                        new StringContent(JsonConvert.SerializeObject(recipe), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, recipe);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(recipe);
            }
        }

        //GET
        public async Task<IActionResult> Edit(int? id)
        {

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var recipe = new Recipe();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recipe = JsonConvert.DeserializeObject<Recipe>(apiResponse);
                }
            }

            if (recipe == null || userId != recipe.AuthorId)
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
                var id = recipe.Id;
                
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }
                recipe.AuthorId = user.Id;

                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"https://localhost:7147/api/Recipe/{id}",
                        new StringContent(JsonConvert.SerializeObject(recipe), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, recipe);
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

        // POST
        /*
        public async Task<IActionResult> PrintPDF(string? body, string? title)
        {
            var file;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Recipe/print?body={body}&title={title}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    file = JsonConvert.DeserializeObject<PrintViewModel>(apiResponse);
                }
            }

            return File(file);
        }
        */
    }
}
