using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Flavorique_Web_App.Data.Migrations;

namespace Flavorique_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HTMLToPDFConverter _htmlToPDFConverter;

        public RecipeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HTMLToPDFConverter htmlToPDFConverter)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _htmlToPDFConverter = htmlToPDFConverter;
        }

        // GET: api/Recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }
            return await _db.Recipes.ToListAsync();
        }

        // GET: api/Recipe/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }
            var recipe = await _db.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return recipe;
        }

        // GET: api/Recipe
        [HttpGet("name")]
        public ActionResult<IEnumerable<string>> GetRecipeName()
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }

            var result = _db.Recipes.Select(i => i.Title).OrderBy(j => j).ToList(); // Convert the result to a list of strings

            return result;
        }

        // GET: api/Recipe/short
        // Shortened version of the recipe, for recommended / other views
        [HttpGet("short")]
        public ActionResult<IEnumerable<object>> GetShortRecipes(int number = 3)
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }

            var result = _db.Recipes
                .Select(i => new
                {
                    Title = i.Title,
                    CreatedDateTime = i.CreatedDateTime.ToString("MMMM d, yyyy"),
                    Body = StripHtmlTags(i.Body).Length > 100 ? StripHtmlTags(i.Body).Substring(0, 100) : StripHtmlTags(i.Body)
                })
                .OrderBy(j => j.Title)
                .Take(number)
                .ToList();

            return result;
        }

        // PUT: api/Recipe/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, Recipe recipe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.GetUserAsync(User);

                var notUpdatedRecipe = await _db.Recipes.FindAsync(id);

                if (user == null || !_signInManager.IsSignedIn(User) || notUpdatedRecipe?.AuthorId != user.Id)
                {
                    return Unauthorized();
                }

                if (id != recipe.Id)
                {
                    return BadRequest();
                }

                var content = recipe.Body.Replace("&nbsp; ", "");

                if (content.Length < 100)
                {
                    return Problem("Recipe body is too short.");
                }

                recipe.Body = content.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
                recipe.Body = recipe.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");

                recipe.AuthorId = user.Id;

                _db.Entry(notUpdatedRecipe).CurrentValues.SetValues(recipe);

                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Recipe
        // JSON shoul have "author" : null to avoid headaches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            if (_db.Recipes == null)
            {
                return Problem("Entity set 'db.Recipes' is null.");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var content = recipe.Body.Replace("&nbsp; ", "");

            if (content.Length < 100)
            {
                ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                return BadRequest();
            }

            recipe.Body = recipe.Body.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
            recipe.Body = recipe.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");

            recipe.AuthorId = user.Id;

            string fillerString = "image widget. Press Enter to type after or press Shift + Enter to type before the widget";
            recipe.Body = recipe.Body.Replace(fillerString, ""); 

            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipes), new { id = recipe.Id }, recipe);
        }

        // DELETE: api/Recipe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Recipe/print
        // Returns a recipe formatted for printing
        [HttpGet("print")]
        public async Task<ActionResult<PrintViewModel>> Print(int id)
        {
            try
            {
                var recipe = await _db.Recipes.FindAsync(id);

                string[] parts = recipe.Body.Split(new string[] { "<p id=\"ingredients\">Ingredients" }, StringSplitOptions.None);

                if (parts.Length == 2)
                {
                    string pattern = @"src=""([^""]*)""";

                    Match match = Regex.Match(recipe.Body, pattern);

                    string image = "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/_9A8c_VZOve3/images/377.png";

                    if (match.Success)
                    {
                        image = match.Value.Substring(5);
                        image = image.Remove(image.Length - 1, 1);
                    }

                    var body = "<p id=\"ingredients\">Ingredients" + parts[1];

                    var author = await _userManager.FindByIdAsync(recipe.AuthorId);

                    var viewModel = new PrintViewModel
                    {
                        Image = image,
                        Title = recipe.Title,
                        Author = author.UserName,
                        Body = body
                    };

                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return NoContent();
        }

        // GET: api/Recipe/printPDF
        // Returns the file for printing
        [HttpGet("PrintPDF")]
        public IActionResult PrintPDF(string body, string title)
        {
            if (string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Invalid request. Body and Title are required.");
            }

            byte[] pdfBytes = _htmlToPDFConverter.ConvertHTMLToPDF(body, title);

            return File(pdfBytes, "application/pdf", title);
        }

        private static string StripHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
        private bool Exists(int id)
        {
            return (_db.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

// Previous MVC Controller

/* 
 
public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HTMLToPDFConverter _htmlToPDFConverter;

        public RecipeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HTMLToPDFConverter htmlToPDFConverter)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _htmlToPDFConverter = htmlToPDFConverter;
        }

        public IActionResult Index()
        {
            IEnumerable<Recipe> obj = _db.Recipes.ToList();
            return View(obj);
        }

        //GET
        public IActionResult Details(int? id)
        {
            var obj = _db.Recipes.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //GET
        public IActionResult Create()
        {
            if (_signInManager.IsSignedIn(User)) {
                return View();
            }
            return RedirectToAction("Index");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe obj)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var content = obj.Body.Replace("&nbsp; ", "");

                if (content.Length < 100)
                {
                    ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                    return View(obj);
                }

                obj.Body = obj.Body.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
                obj.Body = obj.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");

                obj.AuthorId = user.Id;

                string fillerString = "image widget. Press Enter to type after or press Shift + Enter to type before the widget";
                obj.Body = obj.Body.Replace(fillerString, ""); ;

                _db.Recipes.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
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
            var obj = _db.Recipes.Find(id);

            if (obj == null || userId != obj.AuthorId)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe obj)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var content = obj.Body.Replace("&nbsp; ", "");

                if (content.Length < 100)
                {
                    ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                    return View(obj);
                }

                obj.Body = content.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
                obj.Body = obj.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");

                obj.AuthorId = user.Id;

                _db.Recipes.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // POST
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Recipes.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.Recipes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET
        public IActionResult Print(int? id)
        {
            try { 
			    var obj = _db.Recipes.Find(id);

                string[] parts = obj.Body.Split(new string[] { "<p id=\"ingredients\">Ingredients" }, StringSplitOptions.None);
			    
			    if (parts.Length == 2) {

			    	string pattern = @"src=""([^""]*)""";

			    	Match match = Regex.Match(obj.Body, pattern);

                    string image = "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/_9A8c_VZOve3/images/377.png";

                    if (match.Success)
                    {
                        image = match.Value.Substring(5);
                        image = image.Remove(image.Length - 1, 1);
                    }

			    	var body = "<p id=\"ingredients\">Ingredients" + parts[1];

			    	var viewModel = new PrintViewModel
			        {
			        	Image = image,
			        	Title = obj.Title,
			        	Author = _userManager.FindByIdAsync(obj.AuthorId).Result.UserName,
			        	Body = body
                    };

                    return View(viewModel);
			    }

				return RedirectToAction("Details", id);
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index");
			}
		}

		// POST
		public IActionResult PrintPDF(string? body, string? title) 
        {
            byte[] pdfBytes = _htmlToPDFConverter.ConvertHTMLToPDF(body, title);

			return File(pdfBytes, "application/pdf", title);
        }
	} 

 */