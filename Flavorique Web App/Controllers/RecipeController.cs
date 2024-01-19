using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json;
using TXTextControl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Flavorique_Web_App.DTOs;

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
        public async Task<ActionResult<IEnumerable<ShortRecipe>>> GetRecipes(string? sortOrder, string? searchString, int? pageNumber, int pageSize = 5)
        {
            if (_db.Recipes == null)
            {
                return NotFound();
            }
            IEnumerable<ShortRecipe> recipes = await _db.Recipes.Select(i => new ShortRecipe
            {
                Id = i.Id,
                AuthorId = i.AuthorId,
                AuthorName = _userManager.FindByIdAsync(i.AuthorId).Result.UserName,
                Title = i.Title,
                CreatedDateTime = i.CreatedDateTime,
                Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body),
                Image = GetImageFromHtml(i.Body)
            }).ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.Title.ToLower().Contains(searchString.ToLower()) 
                                            || _userManager.Users.Any(u => u.Id == r.AuthorId && u.UserName.ToLower().Contains(searchString.ToLower())));
            }
            int count = recipes.Count();

            switch (sortOrder)
            {
                case "title":
                    recipes = recipes.OrderBy(r => r.Title);
                    break;
                case "titleDesc":
                    recipes = recipes.OrderByDescending(r => r.Title);
                    break;
                case "date":
                    recipes = recipes.OrderBy(r => r.CreatedDateTime);
                    break;
                case "dateDesc":
                    recipes = recipes.OrderByDescending(r => r.CreatedDateTime);
                    break;
                case "idDesc":
                    recipes = recipes.OrderByDescending(r => r.Id);
                    break;
                default:
                    recipes = recipes.OrderBy(r => r.Id);
                    break;
            }

            PaginatedList<ShortRecipe> result = await PaginatedList<ShortRecipe>.CreateAsync(recipes, pageNumber ?? 1, pageSize);

            _logger.LogInformation(result.ToString());

            return Ok(new { data = result, pageIndex = result.PageIndex, totalPages = result.TotalPages, count = count });
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
            //var recipe = await _db.Set<Recipe>().Where(r => r.Id == id).Include(r => r.Comments).FirstOrDefaultAsync();

            if (recipe == null)
            {
                return NotFound();
            }
            return recipe;
        }


	      // GET: api/Recipe/Rating/id
        [HttpGet("rating/{id}")]
        public async Task<IActionResult> GetRecipeRating(int id)
        {
            var ratings = await _db.Set<Comment>().Where(c => c.RecipeId == id).Select(c => c.Rating).ToListAsync();

            if (!ratings.Any())
            {
                return NotFound("This recipe has no rating!");
            }

            return Ok(ratings.Average());
        }
	
        // GET: api/Recipe
        [HttpGet("user/{username}")]
        public async Task<ActionResult> GetRecipesByUserName(string username)
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
                    Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body),
                    Image = GetImageFromHtml(i.Body)
                })
                .OrderByDescending(j => j.CreatedDateTime)
                .Take(3)
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
                    Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body),
                    Image = GetImageFromHtml(i.Body)
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
                    _logger.LogInformation($"User ID: {user.Id}");
                    _logger.LogInformation($"Param id: {id}");
                    _logger.LogInformation($"notUpdatedRecipe Author ID: {notUpdatedRecipe?.AuthorId}");
                    _logger.LogCritical($"Recipe Param id: {recipe.Id}");

                    return Unauthorized("Author Id did not match user Id");
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
            return CreatedAtAction(nameof(GetRecipes), new { id = recipe.Id }, recipe);
        }

        // POST: api/Recipe
        // JSON should have "author" : null to avoid headaches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      public async Task<ActionResult<Recipe>> PostRecipe(CreateRecipeDto recipeDto)
        {
            if (_db == null || _db.Recipes == null)
            {
                return Problem("Entity set 'db.Recipes' is null.");
            }

            if (recipeDto == null)
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

            var content = recipeDto.Body.Replace("&nbsp; ", "");

            if (content.Length < 100)
            {
                ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                return BadRequest(ModelState);
            }

            recipeDto.Body = recipeDto.Body.Replace("<p>Ingredients", "<p id=\"ingredients\">Ingredients");
            recipeDto.Body = recipeDto.Body.Replace("<p>Instructions", "<p id=\"instructions\">Instructions");



			string fillerString = "image widget. Press Enter to type after or press Shift + Enter to type before the widget";
            recipeDto.Body = recipeDto.Body.Replace(fillerString, "");
             
            var tags = _db.Set<Tag>().Where(t => recipeDto.TagIds.Contains(t.Id)).ToList();

            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Body = recipeDto.Body,
                Tags = tags,
                CreatedDateTime = DateTime.Now,
                AuthorId = user.Id,
                Author = user,
            };

            _logger.LogInformation(recipe.AuthorId);
            _logger.LogInformation(user.Id);

            var result = _db.Recipes.Add(recipe);
			await _db.SaveChangesAsync();

            return Ok(result.Entity);
            
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

                    var body = "<p id=\"ingredients\">Ingredients" + parts[1];

                    var image = GetImageFromHtml(recipe.Body);

                    var author = await _userManager.FindByIdAsync(recipe.AuthorId);

                    var viewModel = new PrintViewModel
                    {
                        Image = image,
                        Title = recipe.Title,
                        Author = author.UserName,
                        Body = body
                    };
                    _logger.LogCritical(viewModel.Image);
                    _logger.LogCritical(viewModel.Title);
                    _logger.LogCritical(viewModel.Author);
                    _logger.LogCritical(viewModel.Body);
                    return viewModel;
                }

                return BadRequest("Recipe is missing ingredient list");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("RecipeTag")]
        public async Task<IActionResult> GetRecipeTagTable(int recipeId)
        {
            var model = await _db.RecipeTags.Where(m => m.RecipeId == recipeId).Include(m => m.Tag).ToListAsync();

            return Ok(model);
        }

        [HttpGet("RecipeTag/String")]
        public async Task<IActionResult> GetRecipeTagTableAsString(int recipeId)
        {
            var model = await _db.RecipeTags.Where(m => m.RecipeId == recipeId).Select(m => m.TagId.ToString()).ToListAsync();
            var resultString = string.Join(", ", model);
            return Ok(resultString);
        }

        [HttpPost("RecipeTag")]
        public async Task<IActionResult> PostRecipeTagTable([FromBody] FillRecipeTagTableModel model)
        {
            try
            {

                var recipe = await _db.Recipes.FindAsync(model.RecipeId);
                if (recipe == null)
                {
                    return NotFound();
                }

                var tagsArray = model.TagIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToArray();

                foreach (var tag in tagsArray)
                {
                    var requestModel = new RecipeTag { 
                        RecipeId = model.RecipeId,
                        TagId = int.Parse(tag)
                    };

                    _db.RecipeTags.Add(requestModel);
                    await _db.SaveChangesAsync();

                    _logger.LogWarning($"Tag Value: {tag}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RecipeTag/Update")]
        public async Task<IActionResult> UpdateRecipeTagTable([FromBody] FillRecipeTagTableModel model)
        {
            try
            {
                var recipe = await _db.Recipes.FindAsync(model.RecipeId);
                if (recipe == null)
                {
                    return NotFound();
                }

                var previousTable = await _db.RecipeTags.Where(m => m.RecipeId == model.RecipeId).ToListAsync();

                _db.RecipeTags.RemoveRange(previousTable);
                await _db.SaveChangesAsync();

                var tagsArray = model.TagIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToArray();

                foreach (var tag in tagsArray)
                {
                    var requestModel = new RecipeTag
                    {
                        RecipeId = model.RecipeId,
                        TagId = int.Parse(tag)
                    };

                    _db.RecipeTags.Add(requestModel);
                    await _db.SaveChangesAsync();

                    _logger.LogWarning($"Tag Value: {tag}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static string GetImageFromHtml(string htmlContent)
        {
            string pattern = @"src=""([^""]*)""";
            Match match = Regex.Match(htmlContent, pattern);

            if (match.Success)
            {
                var image = match.Value.Substring(5);
                image = image.Remove(image.Length - 1, 1);
                
                return match.Groups[1].Value;
            }

            // Please make an URL for the Website Logo
            return "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/_9A8c_VZOve3/images/377.png";
            /*
             * This does return the Logo, but since the site is local, it cannot be seen by e-mails
            return "https://localhost:7147/logo";
            */
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
