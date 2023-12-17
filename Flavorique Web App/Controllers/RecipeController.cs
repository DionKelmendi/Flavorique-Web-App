using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.RegularExpressions;

namespace Flavorique_Web_App.Controllers
{
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
}
