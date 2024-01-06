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
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Flavorique_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HTMLToPDFConverter _htmlToPDFConverter;

        public RecipeController(ILogger<RecipeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HTMLToPDFConverter htmlToPDFConverter)
        {
            _logger = logger;
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
        [HttpGet("user/{username}")]
        public async Task<ActionResult> GetRecipeByUserName(string username)
        {
            if (_db.Recipes == null)
            {
                return NotFound();
			}

            var user = await _userManager.FindByNameAsync(username);

			if (user == null)
			{
				return NotFound($"User with username {username} not found.");
			}

            var result = _db.Recipes
                .Where(recipe => recipe.AuthorId == user.Id)
                .Select(i => new ShortRecipe
                {
                    Id = i.Id,
                    Title = i.Title,
                    CreatedDateTime = i.CreatedDateTime,
                    Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body)
                })
                .OrderBy(j => j.CreatedDateTime)
                .ToList();
            _logger.LogInformation($"Result count: {result.Count}");

			return Ok(result);
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
                .Select(i => new ShortRecipe
                {
                    Id = i.Id,
                    Title = i.Title,
                    CreatedDateTime = i.CreatedDateTime,
                    Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body)
                })
                .OrderByDescending(j => j.CreatedDateTime)
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

                if (user == null) {
                    return Unauthorized("User not found.");
                }
                
                var notUpdatedRecipe = await _db.Recipes.FindAsync(id);

                if (notUpdatedRecipe?.AuthorId != user.Id)
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
        // JSON should have "author" : null to avoid headaches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            if (_db == null || _db.Recipes == null)
            {
                return Problem("Entity set 'db.Recipes' is null.");
            }

            if (recipe == null)
            {
                return BadRequest("Recipe object is null.");
            }
            
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogInformation("User is null. Unauthorized access.");
                return Unauthorized(); 
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Body", "Model state is not valid");
                return BadRequest(ModelState);
            }

            var content = recipe.Body.Replace("&nbsp; ", "");

            if (content.Length < 100)
            {
                ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                return BadRequest(ModelState);
            }

            recipe.Body = recipe.Body.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
            recipe.Body = recipe.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");

            recipe.AuthorId = user.Id;

            _logger.LogInformation(recipe.AuthorId);
            _logger.LogInformation(user.Id);

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

        [HttpGet("PrintPDF")]
		public async Task<IActionResult> PrintPDF(int id)
		{
			ActionResult<PrintViewModel> result = await Print(id);

            var printViewModel = result.Value;

            string style = "<style>\r\n\t\t#printContainer {\r\n\t\t\tpadding: 20px;\r\n\t\t\twidth: 800px;\r\n\t\t\tborder: 5px solid gray;\r\n\t\t}\r\n\r\n\t\t#headContainer {\r\n\t\t\tfont-size: 12px;\r\n\t\t}\r\n\r\n\t\timg {\r\n\t\t\tborder: 1px solid black;\r\n\t\t\twidth: 150px;\r\n\t\t\theight: 150px;\r\n\t\t\tmargin: 0;\r\n\t\t\tborder-radius: 50%;\r\n\t\t\tobject-position: center;\r\n\t\t\ttop: 0px;\r\n\t\t}\r\n\r\n\t\ttr {\r\n\t\t\twidth: 100%;\r\n\t\t}\r\n\r\n\t\t#printTable, #bodyTd {\r\n\t\t\tborder: 5px solid gray;\r\n\t\t}\r\n\r\n\t\ttd {\r\n\t\t\tpadding: 20px;\r\n\t\t}\r\n\r\n\t</style>";
            string table = $"<div id=\"printContainer\">\r\n\t\t<table id=\"printTable\">\r\n\t\t\t<tr>\r\n\t\t\t\t<td id=\"headContainer\">\r\n\t\t\t\t\t<h1 id=\"modelTitle\">{printViewModel.Title}</h1>\r\n\t\t\t\t\t<p>Author: {printViewModel.Author}</p>\r\n\t\t\t\t\t<p>Reviews: ⭐⭐⭐⭐⭐</p>\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t<img src=\"{printViewModel.Image}\">\r\n\t\t\t\t</td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td id=\"bodyTd\" colspan=\"2\">\r\n\t\t\t\t\t{printViewModel.Body}\r\n\t\t\t\t</td>\r\n\t\t\t</tr>\r\n\t\t</table>\r\n\t</div>";

			_logger.LogInformation(printViewModel.Body);

			byte[] pdfBytes = _htmlToPDFConverter.ConvertHTMLToPDF(style + table, printViewModel.Title);

			return File(pdfBytes, "application/pdf", printViewModel.Title);
		}

		/*
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
*/

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
